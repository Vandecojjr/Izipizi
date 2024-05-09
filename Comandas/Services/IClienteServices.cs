using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IClienteServices
    {
        Task AddCliente(Cliente cliente);
        Task DeleteCliente(Cliente cliente);
        Task<List<Cliente>> ObterTodos();
        Task<Cliente> GetCliente(Guid Id);
        Task<bool> AtualizarCliente(Cliente cliente);
        Task AtualizaPrazoCliente(decimal total, Guid id);
        Task<byte[]> GerarRelatorioCliente(List<Venda> vendas, decimal faltaReceber);
    }
}
