using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface ITransacaoServices
    {
        Task AddTransacaoAsync(Transacao transacao);
        Task<List<Transacao>> GetAllTrasacoesAsync(Caixa caixa = null);
        Task<List<Transacao>> GetTransacoesByMetodoAsync(Guid metodoId , Caixa caixa);
    }
}
