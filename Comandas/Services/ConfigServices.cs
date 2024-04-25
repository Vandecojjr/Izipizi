
using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class ConfigServices : IConfigServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;

        public ConfigServices(ApplicationDbContext context, CurrentUserService user)
        {
            _context = context;
            _user = user;
        }

        public async Task GerarConfigAsync()
        {
            try
            {
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

                Configuracao config = await _context.configuracoes.FirstOrDefaultAsync(x => x.IdUsuario == userId);
                if (config == null) 
                {
                    config = new();
                    config.IdUsuario = userId;

                    _context.Add(config);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {

            }
        }

        public async Task<Configuracao> ObterConfigAsync()
        {
            try
            {
                await GerarConfigAsync();
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

                Configuracao config = await _context.configuracoes.FirstOrDefaultAsync(x => x.IdUsuario == userId);
                return config;
            }
            catch 
            {
                return null;
            }
        }
    }
}
