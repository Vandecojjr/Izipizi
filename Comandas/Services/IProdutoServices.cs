using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IProdutoServices
    {
        Task AddProdutoAsync(Produto produto);
        Task <List<Produto>> GetAllProdutosAsync();
        Task<Produto> GetProdutosByIdAsync(Guid id);
        Task<Produto> GetProdutosByCodigoAsync(int codigo);
        Task UpdateProdutoAsync(Produto produto, bool volumeQuantidade = false, bool compra = false);
        Task DeleteProdutoAsync(Guid id);
        Task GravaLogProduto(LogPrecoProduto log);
        Task<List<LogPrecoProduto>> GetAllLogs();
        Task<List<LogPrecoProduto>> GetAllLogsPorPeriodo(DateTime? inicial, DateTime? final);
        Task<byte[]> GerarRelatorio(List<Produto> produtos, int tipo = 0);
        Task<byte[]> GerarCodeDeBarras(Produto produto);
        Task<bool> ClonarProdutos(string doador, string receptor);
    }
}
