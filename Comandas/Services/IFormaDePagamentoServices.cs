using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IFormaDePagamentoServices
    {
        Task AddFormaDePAgamento(FormaDePagamento formaDePagamento, Venda venda);
        Task <List<FormaDePagamento>> GetFormaDePagamentos(Venda venda);
    }
}
