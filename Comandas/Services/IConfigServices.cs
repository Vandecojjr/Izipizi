using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface IConfigServices
    {
        Task GerarConfigAsync();
        Task<Configuracao> ObterConfigAsync();
        Task EditarConfig(Configuracao configuracao);
    }
}
