
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

        public async Task AddHistorico(decimal valor, string userId, bool entradaOrNot, Guid IdEmAberto)
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
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddProdutoEmAberto(List<ProdutoVendido> produtos, int comanda, string vendedor)
        {
            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    string userId = await _user.GetCurrentUserIdAsync();
                    var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                    if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;


                    decimal? total = 0;
                    var vendaEmAberto = await _context.VendasEmAberto.FirstOrDefaultAsync(x => x.Numero == comanda && x.IdDoUsuario == userId);
                    if(vendaEmAberto != null) 
                    { 
                        foreach (var item in produtos)
                        {
                            ProdutosEmAberto produto = new();
                            var ajustarEstoque = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                            total += item.valor * item.Quantidade;
                            ajustarEstoque.Quantidade -= item.Quantidade;


                            if (vendedor == null) produto.Vendedor = userCurrent.Email;
                            else produto.Vendedor = vendedor;
                            produto.IdDoProduto = item.IdDoProduto;
                            produto.NomeProduto = item.Nome;
                            produto.NumeroComanda = comanda;
                            produto.Quantidade = item.Quantidade == null ? 0 : (int)item.Quantidade;
                            produto.IdDoUsuario = userId;
                            produto.DataDaVenda = DateTime.Now;
                            produto.total = item.valor * item.Quantidade;

                            _context.Add(produto);
                            await _produtoServices.UpdateProdutoAsync(ajustarEstoque, true);
                        }
                        vendaEmAberto.Total += total == null ? 0 : (decimal)total;
                        _context.Entry(vendaEmAberto).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        await AddHistorico(total == null ? 0 : (decimal)total, userId, true, vendaEmAberto.Id);
                    }
                        transactionScope.Complete(); // Marca a transação como concluída
                }
            }
            catch (Exception)
            {

                throw;
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
                if(Math.Round(venda.Total, 2) <=0) { await DeletarEmAberto(numero); }
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
