using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using static MudBlazor.CategoryTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Comandas.Services
{
    public class VendaServices : IVendaServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly IProdutoVendidoServices _produtoVendidoServices;
        private readonly IFormaDePagamentoServices _formaDePagamentoServices;
        private readonly ITransacaoServices _transacaoServices;
        private readonly IClienteServices _clienteServices;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VendaServices(ApplicationDbContext context, CurrentUserService user, IProdutoVendidoServices produtoVendidoServices, IFormaDePagamentoServices formaDePagamentoServices, ITransacaoServices transacaoServices, IClienteServices clienteServices, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _user = user;
            _produtoVendidoServices = produtoVendidoServices;
            _formaDePagamentoServices = formaDePagamentoServices;
            _transacaoServices = transacaoServices;
            _clienteServices = clienteServices;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> AddVendaAsync(Venda venda, List<ProdutoVendido> produtosVendidos, List<FormaDePagamento> formasDePagamento, Cliente cliente)
        {
            try
            {
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
                var userNome = userCurrent.Email;

                decimal? Conferetroco = formasDePagamento.Sum(x => x.Valor != null ? x.Valor : 0);
                decimal? ajustaLimite = formasDePagamento.Where(x => x.NomeDoMetodo == "A prazo").Sum(x => x.Valor != null ? x.Valor : 0);
                var maiorVendaNumero = await _context.Vendas.Where(x => x.IdUsuario == userId).MaxAsync(x => x.Numero);
                venda.Numero = maiorVendaNumero == 0 ? 1 : maiorVendaNumero + 1;
                venda.IdUsuario = userId;
                venda.NomeDoUsuario = userNome;
                venda.DataDaVenda = DateTime.Now;
                venda.NomeDoCliente = cliente == null ? null : cliente.Nome;
                venda.IdCliente = cliente == null ? null : cliente.Id;
                venda.Lucro = 0;

                // calcula o lucro da venda
                foreach (var item in produtosVendidos)
                {
                    var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                    item.Lucro = (produto.Valor - produto.ValorDeCusto) * item.Quantidade;

                    if (produto.ValorDeCusto == 0 || produto.ValorDeCusto == null) item.Lucro = 0;
                    venda.Lucro += item.Lucro;
                }
                venda.Lucro -= venda.Descontos;
                _context.Add(venda);

                //adiciona os produtos vendidos
                var retornaProdutosOK = await _produtoVendidoServices.AddProdutoVendidoAsync(null, venda.Numero, venda, produtosVendidos);
                if (!retornaProdutosOK) { return false; }

                //se a venda teve formas a prazo ele modifica o limite do cliente
                if (venda.IsPrazo)
                {
                    cliente.LimiteRestante -= ajustaLimite;
                    var resposta =  await _clienteServices.AtualizarCliente(cliente);
                    if (!resposta){return false; }
                }

                //Adiciona formas de pagamento
                formasDePagamento = formasDePagamento.Where(x => x.Valor != null || Math.Round(x.Valor == null ? 0 : (decimal)x.Valor, 2) != 0).ToList();
                var retornoFomas = await _formaDePagamentoServices.AddFormaDePAgamento(null, venda, formasDePagamento);
                if(!retornoFomas) { return false; }

                //transação caso exista troco
                if (Conferetroco > venda.Total)
                {
                    var metodoPadrao = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.IdDoUsuario == userId && x.Padrao == true);
                    Transacao transacao = new Transacao();
                    transacao.Valor = (decimal)Conferetroco - venda.Total;
                    transacao.Tipo = false;
                    transacao.Nome = $"Venda N° {venda.Numero} Troco.";
                    transacao.MetodoId = metodoPadrao.Id;
                    var retornoTransação = await _transacaoServices.AddTransacaoAsync(transacao);
                    if (!retornoTransação) { return false; }
                }

                try
                {
                    
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<List<Venda>> GetAllVendasAsync()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            var vendas = await _context.Vendas.Where(p => p.IdUsuario == userId).ToListAsync();
            return vendas;
        }

        public async Task DeleteVendaAsync(Guid id, Guid IdDometodo)
        {
            var venda = await _context.Vendas.FindAsync(id);
            if (venda != null)
            {
                await _produtoVendidoServices.RemoverProdutoVendido(venda);
                
                
                Transacao transacao = new Transacao();
                transacao.Valor = venda.Total;
                transacao.Tipo = false;
                transacao.Nome = $"Venda N° {venda.Numero} Cancelada.";
                transacao.MetodoId = IdDometodo;
                if (venda.IsPrazo)
                {
                    var metodo = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.Nome == "A prazo");
                    transacao.MetodoId = metodo.Id;
                    var cliente = await _clienteServices.GetCliente((Guid)venda.IdCliente);
                    decimal limiteRestante = 0;
                    var pagamentos = await _formaDePagamentoServices.GetFormaDePagamentos(venda);

                    foreach (var item in pagamentos)
                    {
                        if (item.NomeDoMetodo == "A prazo")
                        {
                            limiteRestante += item.Valor == null ? 0 : (decimal)item.Valor;
                        }
                    }

                    cliente.LimiteRestante += limiteRestante;
                    await _clienteServices.AtualizarCliente(cliente);

                }
                await _transacaoServices.AddTransacaoAsync(transacao);
                _context.Vendas.Remove(venda);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<Venda>> ObterVendasPorDataAsync(DateTime? inicial, DateTime? final)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            return await _context.Vendas.Where(x => x.DataDaVenda.Date >= inicial && x.DataDaVenda.Date <= final && x.IdUsuario == userId).ToListAsync();
        }

        public async Task<List<decimal>> ObterVendasDosMeses()
        {
            List<decimal> totalDosMeses = new List<decimal>();
            List<Venda> vendas = new List<Venda>();
            

            for (int i = 1; i < 13; i++)
            {
                DateTime dataAtual = DateTime.Now;
                DateTime? inicialChar = new DateTime(dataAtual.Year, i, 1);
                DateTime? FinalChar = new DateTime(dataAtual.Year, i, DateTime.DaysInMonth(dataAtual.Year, i));

                string userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
                vendas = await _context.Vendas.Where(x => x.DataDaVenda.Date >= inicialChar && x.DataDaVenda.Date <= FinalChar && x.IdUsuario == userId).ToListAsync();
                decimal total = 0;
                foreach (var item in vendas)
                {
                    total += item.Total;
                }
                if(vendas.Count == 0) total = 0;
                totalDosMeses.Add(total);
            }
            return totalDosMeses;
        }

        public Task AtualizaVendaCliente(Venda venda)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GerarRelatorioVendas(List<Venda> vendas)
        {
            List<RelatorioVendas> VendasRelatorio = new List<RelatorioVendas>();
            foreach (var item in vendas)
            {
                RelatorioVendas novoRelatorio = new();
                novoRelatorio.Numero = item.Numero;
                novoRelatorio.Total = item.Total;
                novoRelatorio.Descontos = item.Descontos;
                novoRelatorio.Lucro = item.Lucro;
                novoRelatorio.NomeDoUsuario = item.NomeDoUsuario;
                novoRelatorio.NomeDoCliente = item.NomeDoCliente;
                novoRelatorio.Data = item.DataDaVenda;
                VendasRelatorio.Add(novoRelatorio);
            }
            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\vendas_relatorio.frx");
            var Freport = new FastReport.Report();
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(VendasRelatorio, "vendas", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }

        public async Task<byte[]> GerarRecibo(List<ProdutoVendido> produtoVendidos, decimal desconto = 0, decimal total = 0)
        {
            List<ProdutoVendidoRelatorio> produtosRecibo = new List<ProdutoVendidoRelatorio>();
            ProdutoVendidoRelatorio novoRelatorio = new();
            foreach (var item in produtoVendidos)
            {
                
                var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                novoRelatorio.Nome  = item.Nome;
                novoRelatorio.Lucro = produto.Valor;
                novoRelatorio.Quantidade = item.Quantidade;
                produtosRecibo.Add(novoRelatorio);
                novoRelatorio = new();
            }
            novoRelatorio.Total = total;
            novoRelatorio.Desconto = desconto;
            produtosRecibo.Add(novoRelatorio);

            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\recibo.frx");
            var Freport = new FastReport.Report();

            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(produtosRecibo, "recibo", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }
    }
}
