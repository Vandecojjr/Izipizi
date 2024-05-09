using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IFormaDePagamentoServices
    {
        Task<bool> AddFormaDePAgamento(FormaDePagamento? formaDePagamento = null, Venda? venda = null, List<FormaDePagamento>? formas = null);
        Task <List<FormaDePagamento>> GetFormaDePagamentos(Venda venda);
    }
}
