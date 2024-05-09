using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IProdutoVendidoServices
    {
        Task<bool> AddProdutoVendidoAsync(ProdutoVendido? produtoVendido = null, int numeroDaVenda = 0, Venda venda = null, List<ProdutoVendido>? produtos = null);
        Task RemoverProdutoVendido(Venda venda);
        Task<List<ProdutoVendido>> GetProdutosVendidosAsync(Venda venda);
        Task<List<Despesa>> TotalDeDispesasPorPeriodo(DateTime? inicial, DateTime? final);

    }
}
