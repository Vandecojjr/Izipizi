using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class CategoriaServices : ICategoriaServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;

        public CategoriaServices(ApplicationDbContext context, CurrentUserService user)
        {
            _context = context;
            _user = user;

        }

        public async Task AddCategoriaAsync(Categoria categoria)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            categoria.ApplicationUserId = userId;
            _context.Add(categoria);
            _context.SaveChanges();
        }

        public async Task DeleteCategoriaAsync(Guid CategoriaId)
        {
            var categoria = await _context.Categorias.FindAsync(CategoriaId);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Categoria>> GetAllCategoriasAsync()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            var categorias = await _context.Categorias.Where(p => p.ApplicationUserId == userId).ToListAsync();
            return categorias;
        }

        public async Task<Categoria> GetCategoriasByIdAsync(Guid CategoriaId)
        {
            return await _context.Categorias.FirstOrDefaultAsync(p => p.CategoriaId == CategoriaId);
        }

        public async Task UpdateCategoriaAsync(Categoria categoria)
        {
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
