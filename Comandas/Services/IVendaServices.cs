using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IVendaServices
    {
        Task AddVendaAsync(Venda venda, List<ProdutoVendido> produtosVendidos, List<FormaDePagamento> formasDePagamento);
        Task DeleteVendaAsync(Guid id, Guid IdDometodo);
        Task<List<Venda>> GetAllVendasAsync();
        Task<List<Venda>> ObterVendasPorDataAsync(DateTime? inicial, DateTime? final);
        Task<List<decimal>> ObterVendasDosMeses();
        Task AtualizaVendaCliente(Venda venda);
        Task<byte[]> GerarRelatorioVendas(List<Venda> vendas);
        Task<byte[]> GerarRecibo(List<ProdutoVendido> produtoVendidos, decimal desconto = 0, decimal total = 0 );
    }
}
