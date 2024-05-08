using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IProdutoVendidoServices
    {
        Task AddProdutoVendidoAsync(ProdutoVendido produtoVendido, int numeroDaVenda = 0, Venda venda = null);
        Task RemoverProdutoVendido(Venda venda);
        Task<List<ProdutoVendido>> GetProdutosVendidosAsync(Venda venda);
        Task<List<Despesa>> TotalDeDispesasPorPeriodo(DateTime? inicial, DateTime? final);

    }
}
