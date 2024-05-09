using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class TransacaoServices : ITransacaoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly IMetodoDePagamentoServices _metodoDePagamentoServices;

        public TransacaoServices(ApplicationDbContext context, CurrentUserService user, IMetodoDePagamentoServices metodoDePagamentoServices)
        {
            _context = context;
            _user = user;
            _metodoDePagamentoServices = metodoDePagamentoServices;
        }

        public async Task<bool> AddTransacaoAsync(Transacao transacao)
        {
            try
            {
                var userId = await _user.GetCurrentUserIdAsync();
                var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
                var userNome = await _user.GetCurrentUserNameAsync();
                var metodoNome = (await _metodoDePagamentoServices.GetMetodoDePagamentoAsync(transacao.MetodoId)).Nome;
                Caixa caixaAtual = await GetCaixaAberto();

                if (caixaAtual != null)
                {
                    transacao.UserId = userId;
                    transacao.UserNome = userNome;
                    transacao.Data = DateTime.Now;
                    transacao.Caixa = caixaAtual;
                    transacao.MetodoNome = metodoNome;

                    _context.Add(transacao);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<List<Transacao>> GetAllTrasacoesAsync(Caixa caixa)
        {
            Caixa caixaAtual = new();
            if (caixa == null)
            {
                caixaAtual = await GetCaixaAberto();
            }
            else
            {
                caixaAtual = caixa;
            }
            return await _context.transacoes.Where(x => x.Caixa == caixaAtual).ToListAsync();
        }

        public async Task<List<Transacao>> GetTransacoesByMetodoAsync(Guid metodoId, Caixa caixa)
        {
            var transacoes  = await _context.transacoes.Where(x => x.MetodoId == metodoId && x.Caixa == caixa).ToListAsync();

            return transacoes;
        }


        public async Task<Caixa> GetCaixaAberto()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            return await _context.Caixas.FirstOrDefaultAsync(x => x.UsuarioId == userId && x.Estado == true);
        }
    }
}
