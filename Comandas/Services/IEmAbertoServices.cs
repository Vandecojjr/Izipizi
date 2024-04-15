using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IEmAbertoServices
    {
        Task<List<EmAberto>> GetAllEmAberto();
        Task AddEmAberto(EmAberto emAberto);
        Task AddProdutoEmAberto(List<ProdutoVendido> produtos, int comanda, string vendedor);
        Task RemoveProdutoEmAberto(List<ProdutosEmAberto> produtos, int comanda);
        Task<List<ProdutosEmAberto>> GetAllProdutosEmAberto(int numero);
        Task DeletarEmAberto(int numero);
        Task EditarComanda(int numero, decimal valor);
    }
}
