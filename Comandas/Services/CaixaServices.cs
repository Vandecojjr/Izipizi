using Comandas.Components.Pages.Produtos;
using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class CaixaServices : ICaixaServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly ITransacaoServices _transacaoService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CaixaServices(ApplicationDbContext context, CurrentUserService user, ITransacaoServices transacaoService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _user = user;
            _transacaoService = transacaoService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task AbrirCaixa(Transacao abertura)
        {
            var confereCaixa = await GetCaixaAberto();
            if (confereCaixa == null)
            {
                string userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
                Caixa caixa = new Caixa();
                abertura.Nome = "Abertura";
                abertura.Tipo = true;

                caixa.Total = 0;
                caixa.TotalDeSaida = 0;
                caixa.TotalDeEntrada = 0;
                caixa.Abertura = abertura.Valor;
                caixa.DataDeAbertura = DateTime.Now;
                caixa.UsuarioId = userId;
                caixa.Estado = true;

                _context.Add(caixa);
                await _context.SaveChangesAsync();
                
                await _transacaoService.AddTransacaoAsync(abertura);
            }
        }

        public async Task FecharCaixa(List<FechamentoCaixa> fechamento, decimal total, decimal saida, decimal entrada)
        {
            var caixaAtual = await GetCaixaAberto();
            foreach (var item in fechamento)
            {
                _context.Add(item);
            }
            
            caixaAtual.Estado = false;
            caixaAtual.DataDeFechamento = DateTime.Now;
            caixaAtual.Total = total;
            caixaAtual.TotalDeSaida = saida;
            caixaAtual.TotalDeEntrada = entrada;
            _context.Entry(caixaAtual).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<byte[]> GerarReciboPagamentoAprazo(decimal totalReceber, decimal total)
        {
            List<RelatorioFechamentoDeConta> fechamentoConta = new List<RelatorioFechamentoDeConta>();
            RelatorioFechamentoDeConta fechamento = new();
            fechamento.receber = total;
            fechamento.Conta = totalReceber;
            fechamentoConta.Add(fechamento);

            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\fechar_conta_relatorio.frx");
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

        public async Task<byte[]> GerarRelatorioApuracao(List<FechamentoCaixa> fechamentos)
        {
            List<RelatorioApuracao> fechamentosRelatorio = new List<RelatorioApuracao>();
            foreach (var item in fechamentos)
            {
                RelatorioApuracao novoRelatorio = new();
                novoRelatorio.NomeMetodo = item.NomeMetodo;
                novoRelatorio.ValorApurado = item.ValorApurado;
                novoRelatorio.ValorInformado = item.ValorInformado;
                novoRelatorio.ValorDiferenca =  item.ValorInformado - item.ValorApurado;
                fechamentosRelatorio.Add(novoRelatorio);
            }
            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\apuracao_caixa_relatorio.frx");
            var Freport = new FastReport.Report();
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(fechamentosRelatorio, "fechamentos", 10, true);
            Freport.Prepare();

            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();

        }

        public async Task<byte[]> GerarRelatorioTransaoes(List<Transacao> transacoes)
        {
            List<RelatorioDeTransacoesCaixa> transacoesRelatorio = new List<RelatorioDeTransacoesCaixa>();
            foreach (var item in transacoes)
            {
                RelatorioDeTransacoesCaixa novoRelatorio = new();
                novoRelatorio.Nome = item.Nome;
                novoRelatorio.UserNome = item.UserNome;
                novoRelatorio.Data = item.Data;
                if (item.Tipo)
                {
                    novoRelatorio.Tipo ="Entrada";
                }
                else
                {
                    novoRelatorio.Tipo = "Saida";
                }
                novoRelatorio.Valor = item.Valor;
                novoRelatorio.MetodoNome = item.MetodoNome;
                transacoesRelatorio.Add(novoRelatorio);
            }
            var reportFile = Path.Combine(_webHostEnvironment.WebRootPath, @"relatorios\trasacoes_caixa_relatorio.frx");
            var Freport = new FastReport.Report();
            Freport.Load(reportFile);
            Freport.Report.Dictionary.RegisterBusinessObject(transacoesRelatorio, "transacoes", 10, true);
            Freport.Prepare();



            var pdfExport = new PDFSimpleExport();
            using MemoryStream memory = new MemoryStream();

            pdfExport.Export(Freport, memory);
            memory.Flush();

            return memory.ToArray();
        }

        public async Task<List<Caixa>> GetAllCaixas()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            return await _context.Caixas.Where(x => x.UsuarioId == userId).ToListAsync();
        }

        public async Task<List<FechamentoCaixa>> GetAllFechamentos(Caixa caixa)
        {
            return await _context.fechamentoCaixas.Where(x => x.Caixa == caixa).ToListAsync();
        }

        public async Task<Caixa> GetCaixaAberto()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            return await _context.Caixas.FirstOrDefaultAsync(x => x.UsuarioId == userId && x.Estado == true);
        }

        public async Task<List<Caixa>> ObterPorData(DateTime? inicial, DateTime? final)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            return await _context.Caixas.Where(x => x.DataDeAbertura.Date >= inicial && x.DataDeAbertura.Date <= final && x.UsuarioId == userId).ToListAsync();
        }

        public async Task ReabrirCaixa(Caixa caixa)
        {
            var caixaAtual = await GetCaixaAberto();
            if (caixaAtual == null) 
            {
                List<FechamentoCaixa> fechamentos = await _context.fechamentoCaixas.Where(x => x.Caixa == caixa).ToListAsync();
                foreach (var item in fechamentos)
                {
                    _context.Remove(item);
                }
                caixa.Estado = true;
                _context.Entry(caixa).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
