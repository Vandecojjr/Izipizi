using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IProdutoVendidoServices
    {
        Task AddProdutoVendidoAsync(ProdutoVendido produtoVendido, int numeroDaVenda, Venda venda);
        Task RemoverProdutoVendido(Venda venda);
        Task<List<ProdutoVendido>> GetProdutosVendidosAsync(Venda venda);

    }
}
