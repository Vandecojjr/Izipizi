using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class EmAbertoServices : IEmAbertoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;

        public EmAbertoServices(ApplicationDbContext context, CurrentUserService user)
        {
            _context = context;
            _user = user;
        }

        public async Task AddEmAberto(EmAberto emAberto)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            emAberto.IdDoUsuario = userId;
            emAberto.NomeVendedor = userCurrent.Email;
            emAberto.Total = 0;
            emAberto.DataDeAbertura = DateTime.Now;

            _context.Add(emAberto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<EmAberto>> GetAllEmAberto()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var EmAbertos = await _context.VendasEmAberto.Where(x => x.IdDoUsuario == userId).ToListAsync();
            return EmAbertos;
        }
    }
}
