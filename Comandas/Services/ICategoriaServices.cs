using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface ICategoriaServices
    {
        Task<List<Categoria>> GetAllCategoriasAsync();
        Task AddCategoriaAsync(Categoria categoria);
        Task DeleteCategoriaAsync(Guid CategoriaId);
        Task<Categoria> GetCategoriasByIdAsync(Guid CategoriaId);
        Task UpdateCategoriaAsync(Categoria categoria);
    }
}
