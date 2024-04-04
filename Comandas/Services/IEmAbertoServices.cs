using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IEmAbertoServices
    {
        Task<List<EmAberto>> GetAllEmAberto();
        Task AddEmAberto(EmAberto emAberto);
    }
}
