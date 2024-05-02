using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface ICaixaServices
    {
        Task AbrirCaixa(Transacao abertura);
        Task<Caixa> GetCaixaAberto();
        Task FecharCaixa(List<FechamentoCaixa> fechamento, decimal total, decimal saida, decimal entrada);
        Task<List<Caixa>> GetAllCaixas();
        Task<List<Caixa>> ObterPorData(DateTime? inicial, DateTime? final);
        Task<List<FechamentoCaixa>> GetAllFechamentos(Caixa caixa);
        Task ReabrirCaixa(Caixa caixa);
        Task<byte[]> GerarRelatorioApuracao(List<FechamentoCaixa> fechamentos);
        Task<byte[]> GerarRelatorioTransaoes(List<Transacao> transacoes);
        Task<byte[]> GerarReciboPagamentoAprazo(decimal totalReceber, decimal total);
    }
}
