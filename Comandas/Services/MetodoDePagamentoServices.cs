using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class MetodoDePagamentoServices : IMetodoDePagamentoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;

        public MetodoDePagamentoServices(ApplicationDbContext context , CurrentUserService user)
        {
            _user = user;
            _context = context;
        }

        public async Task AddMetodoAsync(MetodoDePagamento metodo)
        {
            var userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            metodo.IdDoUsuario = userId;
            if (userCurrent.nivelAdmin == 2) metodo.IdDoUsuario = userCurrent.IdDoProprietario;

            if(metodo.Padrao)
            {
                var metodos = await GetAllMetodosAsync();
                foreach (var item in metodos)
                {
                    if(item.Padrao)
                    {
                        item.Padrao = false;
                        await EditarMetodo(item);
                    }
                }
            }
            _context.Add(metodo);
            await _context.SaveChangesAsync();
        }

        public async Task Deletar(Guid id)
        {
            var metodo = await GetMetodoDePagamentoAsync(id);
            if(metodo.Padrao)
            {
                string userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

                var novoMetodoPadrao = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.Nome == "Dinheiro" && x.IdDoUsuario == userId);
                novoMetodoPadrao.Padrao = true;
                await EditarMetodo(novoMetodoPadrao);
            }
            _context.MetodosDePagamento.Remove(metodo);
            await _context.SaveChangesAsync();
        }

        public async Task EditarMetodo(MetodoDePagamento metodo)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            var metodoPadraoAtual = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.Padrao == true && x.IdDoUsuario == userId);
            if (metodo.Padrao)
            {
                if (metodoPadraoAtual != null)
                {
                    metodoPadraoAtual.Padrao = false;
                    _context.Entry(metodoPadraoAtual).State = EntityState.Modified;
                }
            }
            _context.Entry(metodo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var metodos = await GetAllMetodosAsync();

                var testePadrao = false;
                foreach (var item in metodos)
                {
                    if (item.Padrao)
                    {
                        testePadrao = true;
                    }
                }
                if (!testePadrao) 
                {
                    var novoMetodoPadrao = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.Nome == "Dinheiro" && x.IdDoUsuario == userId);
                    novoMetodoPadrao.Padrao = true;
                    await EditarMetodo(novoMetodoPadrao);
                }

            
        }

        public async Task<List<MetodoDePagamento>> GetAllMetodosAsync()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var metodos = await _context.MetodosDePagamento.Where(p => p.IdDoUsuario == userId).ToListAsync();
            return metodos;
        }

        public async Task<MetodoDePagamento> GetMetodoDePagamentoAsync(Guid id)
        {
            var metodo = await _context.MetodosDePagamento.FirstOrDefaultAsync(x => x.Id == id);
            return metodo;
        }

        public async Task MetodosIniciais()
        {
            List<MetodoDePagamento> metodos = await GetAllMetodosAsync();
            if (metodos.Count == 0)
            {
                MetodoDePagamento metodo = new MetodoDePagamento();
                metodo.Nome = "Dinheiro";
                metodo.Padrao = true;
                await AddMetodoAsync(metodo);

                metodo = new MetodoDePagamento();
                metodo.Nome = "Pix";
                metodo.Padrao = false;
                await AddMetodoAsync(metodo);

                metodo = new MetodoDePagamento();
                metodo.Nome = "Credito";
                metodo.Padrao = false;
                await AddMetodoAsync(metodo);

                metodo = new MetodoDePagamento();
                metodo.Nome = "Debito";
                metodo.Padrao = false;
                await AddMetodoAsync(metodo);

                metodo = new MetodoDePagamento();
                metodo.Nome = "A prazo";
                metodo.Padrao = false;
                await AddMetodoAsync(metodo);
            }
        }
    }
}
