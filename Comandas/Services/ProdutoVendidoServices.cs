using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using MercadoPago.Resource.User;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class ProdutoVendidoServices : IProdutoVendidoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IProdutoServices _produtoServices;
        private readonly CurrentUserService _user;

        public ProdutoVendidoServices(ApplicationDbContext context, IProdutoServices produtoServices, CurrentUserService user)
        {
            _context = context;
            _produtoServices = produtoServices;
            _user = user;
        }

        public async Task<bool> AddProdutoVendidoAsync(ProdutoVendido? produtoVendido =  null, int numeroDaVenda = 0, Venda venda = null, List<ProdutoVendido>? produtos = null)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            try
            {
                if (produtos == null) 
                {   
                    produtos = new();
                    produtos.Add(produtoVendido); 
                }
                foreach (var item in produtos)
                {
                    var produtoAntigo = await _produtoServices.GetProdutosByIdAsync(item.IdDoProduto);
                    var quantidade = item.Quantidade;
                    if (produtoAntigo.IsVolume)
                    {
                        var codigo = produtoAntigo.CodigoDoProdutoVolume;
                        quantidade = produtoAntigo.QuantidadeVolume * quantidade;
                        produtoAntigo = await _produtoServices.GetProdutosByIdAsync((Guid)codigo);
                    }
                    produtoAntigo.Quantidade -= quantidade;
                    item.NumeroDaVenda = numeroDaVenda;
                    item.Venda = venda;
                    item.IdDoUsuario = userId;
                    if (item.Despesa) item.DespesaDate = DateTime.Now;
                    await _produtoServices.UpdateProdutoAsync(produtoAntigo);
                }

                _context.AddRange(produtos);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<List<ProdutoVendido>> GetProdutosVendidosAsync(Venda venda)
        {
            var produtosVendidos = await _context.ProdutosVendidos.Where(x => x.Venda == venda).ToListAsync();
            return produtosVendidos;
        }

        public async Task RemoverProdutoVendido(Venda venda)
        {
            List<ProdutoVendido> produtosVendidos = await _context.ProdutosVendidos.Where(x => x.Venda == venda).ToListAsync();

            foreach (var item in produtosVendidos)
            {
                var produtoAntigo = await _produtoServices.GetProdutosByIdAsync(item.IdDoProduto);
                var quantidade = item.Quantidade;
                if (produtoAntigo != null)
                {
                    if (produtoAntigo.IsVolume)
                    {
                        var codigo = produtoAntigo.CodigoDoProdutoVolume;
                        quantidade = produtoAntigo.QuantidadeVolume * quantidade;
                        produtoAntigo = await _produtoServices.GetProdutosByIdAsync((Guid)codigo);
                    }
                    produtoAntigo.Quantidade += quantidade;
                    await _produtoServices.UpdateProdutoAsync(produtoAntigo);
                }
            }
        }

        public async Task<List<Despesa>> TotalDeDispesasPorPeriodo(DateTime? inicial, DateTime? final)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var produtosVendido = await _context.ProdutosVendidos.Where(x => x.DespesaDate.Date >= inicial && x.DespesaDate.Date <= final && x.IdDoUsuario == userId).ToListAsync();
            var transacoes = await _context.transacoes.Where(x => x.Data.Date >= inicial && x.Data.Date <= final && x.UserId == userId && x.Despesa).ToListAsync();
            List<Despesa> despesas = new List<Despesa>();

            foreach (var item in produtosVendido)
            {
                Despesa despesa = new Despesa();
                despesa.Nome = item.Nome;
                despesa.DataDespesa = item.DespesaDate;
                despesa.Valor = (decimal)item.valor;
                despesas.Add(despesa);
            }

            foreach (var item in transacoes)
            {
                Despesa despesa = new Despesa();
                despesa.Nome = item.Nome;
                despesa.DataDespesa = item.Data;
                despesa.Valor = (decimal)item.Valor;
                despesas.Add(despesa);
            }
            return despesas;
        }
    }
}
