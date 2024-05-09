
using Comandas.Data;
using Comandas.Data.Models;
using FastReport.Data;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Comandas.Services
{
    public class EmAbertoServices : IEmAbertoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly CurrentUserService _user;
        private readonly IProdutoServices _produtoServices;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public EmAbertoServices(ApplicationDbContext context, CurrentUserService user, IProdutoServices produtoServices)
        {
            _context = context;
            _user = user;
            _produtoServices = produtoServices;
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

        public async Task<bool> AddHistorico(decimal valor, string userId, bool entradaOrNot, Guid IdEmAberto)
        {
            try
            {
                HistoricoEmAberto historicoNovo = new();
                historicoNovo.IsEntrada = entradaOrNot;
                historicoNovo.IdEmAberto = IdEmAberto;
                historicoNovo.Horario = DateTime.Now;
                historicoNovo.UserID = userId;
                historicoNovo.Valor = valor;
                _context.Add(historicoNovo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> AddProdutoEmAberto(List<ProdutoVendido> produtos, EmAberto comanda, string vendedor)
        {
            await _semaphore.WaitAsync();
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    string userId = await _user.GetCurrentUserIdAsync();
                    var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;


                    decimal? total = produtos.Sum(x => x.valor * x.Quantidade);
                    if(comanda != null) 
                    {
                        if(total == null) return false;
                        comanda.Total += total == null ? 0 : (decimal)total;
                        try
                        {
                            _context.Entry(comanda).State = EntityState.Modified;
                        }
                        catch (Exception)
                        {
                            return false;
                            throw;
                        }

                        try
                        {
                            foreach (var item in produtos)
                            {
                                ProdutosEmAberto produto = new();
                                var ajustarEstoque = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                                ajustarEstoque.Quantidade -= item.Quantidade;


                                if (vendedor == null) produto.Vendedor = userCurrent.Email;
                                else produto.Vendedor = vendedor;
                                produto.IdDoProduto = item.IdDoProduto;
                                produto.NomeProduto = item.Nome;
                                produto.NumeroComanda = comanda.Numero;
                                produto.Quantidade = item.Quantidade == null ? 0 : (int)item.Quantidade;
                                produto.IdDoUsuario = userId;
                                produto.DataDaVenda = DateTime.Now;
                                produto.total = item.valor * item.Quantidade;

                                _context.Add(produto);
                                await _produtoServices.UpdateProdutoAsync(ajustarEstoque, true);
                            }
                        }
                        catch (Exception)
                        {
                            return false;
                            throw;
                        }

                        var retornoHistorico = await AddHistorico(total == null ? 0 : (decimal)total, userId, true, comanda.Id);
                        if (!retornoHistorico) { return false; }
                        await _context.SaveChangesAsync();
                        
                    }
                        transactionScope.Complete(); // Marca a transação como concluída
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            finally
            {
                _semaphore.Release(); // Libera o semáforo após a conclusão do método
            }
        }

        public async Task DeletarEmAberto(int numero)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;


            var produtosEmAbertos = await GetAllProdutosEmAberto(numero);
            var comanda = await _context.VendasEmAberto.FirstOrDefaultAsync(x => x.Numero == numero && x.IdDoUsuario == userId);

            foreach (var item in produtosEmAbertos)
            {
                var ajustarEstoque = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                ajustarEstoque.Quantidade += item.Quantidade;
                if (item.IdDoUsuario == userId && item.NumeroComanda == numero) _context.produtosEmAberto.Remove(item);
                await _produtoServices.UpdateProdutoAsync(ajustarEstoque,true);
            }
            _context.VendasEmAberto.Remove(comanda);
            await _context.SaveChangesAsync();

        }

        public async Task EditarComanda(int numero, decimal valor)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var venda = await _context.VendasEmAberto.FirstOrDefaultAsync(x => x.Numero == numero && x.IdDoUsuario == userId);
            if(venda != null)
            {
                venda.Total -= valor;
                if(Math.Round(venda.Total, 2) <= 0) { await DeletarEmAberto(numero); }
                else
                {
                    _context.Entry(venda).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            await AddHistorico((decimal)valor, userId, false, venda.Id);
        }

        public async Task<List<EmAberto>> GetAllEmAberto()
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var EmAbertos = await _context.VendasEmAberto.Where(x => x.IdDoUsuario == userId).ToListAsync();
            return EmAbertos;
        }

        public async Task<List<ProdutosEmAberto>> GetAllProdutosEmAberto(int numero)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var produtosEmAbertos = await _context.produtosEmAberto.Where(x => x.IdDoUsuario == userId && x.NumeroComanda == numero).ToListAsync();
            return produtosEmAbertos;
        }

        public  async Task<List<HistoricoEmAberto>> GetHistorico(Guid idComanda)
        {
            try
            {
                var historicos = await _context.historicoEmAbertos.Where(x => x.IdEmAberto == idComanda).ToListAsync();
                return historicos;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProdutoEmAberto(List<ProdutosEmAberto> produtos, int comanda)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            decimal? total = 0;
            // Calcular o total para itens com total nulo e verificar a existência do produto
            foreach (var item in produtos)
            {
                var prod = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                if (prod != null)
                {
                    if (item.total == 0 || item.total == null) item.total = prod.Valor * item.Quantidade;
                    prod.Quantidade += 1;
                    await _produtoServices.UpdateProdutoAsync(prod, true);
                }
                else
                {
                    item.total = 0;
                }
                total += item.total;

                // Remover os produtos em aberto
                _context.produtosEmAberto.Remove(item);
            }
            // Salvar as alterações no banco de dados
            await _context.SaveChangesAsync();
            var vendaEmAberto = await _context.VendasEmAberto.FirstOrDefaultAsync(x => x.Numero == comanda && x.IdDoUsuario == userId);
            await EditarComanda(comanda, (decimal)total);
        }
    }
}
