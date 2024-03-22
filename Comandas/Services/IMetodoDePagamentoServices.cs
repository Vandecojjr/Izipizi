using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IMetodoDePagamentoServices
    {
        Task AddMetodoAsync(MetodoDePagamento metodo);
        Task EditarMetodo(MetodoDePagamento metodo);
        Task Deletar(Guid id);
        Task<List<MetodoDePagamento>> GetAllMetodosAsync();
        Task<MetodoDePagamento> GetMetodoDePagamentoAsync(Guid id);
        Task MetodosIniciais();
    }
}
