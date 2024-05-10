using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IEmAbertoServices
    {
        Task<List<EmAberto>> GetAllEmAberto();
        Task AddEmAberto(EmAberto emAberto);
        Task<bool> AddProdutoEmAberto(List<ProdutoVendido> produtos, EmAberto comanda, string vendedor);
        Task RemoveProdutoEmAberto(List<ProdutosEmAberto> produtos, int comanda);
        Task<List<ProdutosEmAberto>> GetAllProdutosEmAberto(int numero);
        Task DeletarEmAberto(int numero);
        Task EditarComanda(int numero, decimal valor);
        Task<bool> AddHistorico(decimal valor, string userId, bool entradaOrNot, Guid IdEmAberto);
        Task<List<HistoricoEmAberto>> GetHistorico(Guid idComanda);
        Task<List<HistoricoEmAberto>> GetHistoricos();
    }
}
