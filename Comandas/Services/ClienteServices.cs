using Comandas.Components.Pages.Categorias;
using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using Microsoft.EntityFrameworkCore;
using System;
using static MudBlazor.CategoryTypes;

namespace Comandas.Services
{
    public class ClienteServices : IClienteServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ClienteServices(ApplicationDbContext context, CurrentUserService user, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _user = user;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task AddCliente(Cliente cliente)
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            cliente.IdUsuario = userId;

            if (cliente.Limite == null || cliente.Limite == 0)
            {
                cliente.Limite = 0;
                cliente.LimiteRestante = 0;
            }
            else
            {
                cliente.LimiteRestante = cliente.Limite;
            }
            cliente.LimiteAntigo = cliente.Limite;
            _context.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizaPrazoCliente(decimal total, Guid id)
        {
            var cliente = await GetCliente(id);
            if (cliente != null)
            {
                cliente.LimiteRestante += total;
                await AtualizarCliente(cliente);
                if ((cliente.Limite - cliente.LimiteRestante) <= 0)
                {
                    var vendas = await _context.Vendas.Where(x => x.IdCliente == cliente.Id && x.IsPrazo == true && x.IsPago == false).ToListAsync();
                    foreach (var item in vendas)
                    {
                        item.IsPago = true;
                        _context.Entry(item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task AtualizarCliente(Cliente cliente)
        {
            

            if (cliente.Limite == null || cliente.Limite == 0)
            {
                cliente.Limite = 0;
                cliente.LimiteRestante = 0;
            }
            else
            {
                cliente.LimiteRestante = cliente.Limite - (cliente.LimiteAntigo - cliente.LimiteRestante) ;
            }
            cliente.LimiteAntigo = cliente.Limite;
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCliente(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> GerarRelatorioCliente(List<Venda> vendas, decimal faltaReceber)
        {
            List<RelatorioVendas> fechamentoConta = new List<RelatorioVendas>();
            RelatorioVendas novoRelatorio = new();
            foreach (var item in vendas)
            {
                
                novoRelatorio.Numero = item.Numero;
                novoRelatorio.Total = item.Total;
                novoRelatorio.Descontos = item.Descontos;
                novoRelatorio.Lucro = item.Lucro;
                novoRelatorio.NomeDoUsuario = item.NomeDoUsuario;
                novoRelatorio.NomeDoCliente = item.NomeDoCliente;
                novoRelatorio.Data = item.DataDaVenda;
                fechamentoConta.Add(novoRelatorio);
                novoRelatorio = new();
            }
            novoRelatorio.TotalCliente = faltaReceber;
            fechamentoConta.Add(novoRelatorio);
            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\cliente_relatorio.frx");
            var Freport = new FastReport.Report();
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(fechamentoConta, "fechamentos", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }

        public async Task<Cliente> GetCliente(Guid Id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(x => Id == x.Id);
            return cliente;
        }

        public async Task<List<Cliente>> ObterTodos()
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var clientes = await _context.Clientes.Where(x => x.IdUsuario == userId).ToListAsync();
            return clientes;
        }
    }
}
