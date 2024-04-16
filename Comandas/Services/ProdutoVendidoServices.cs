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

        public async Task AddProdutoVendidoAsync(ProdutoVendido produtoVendido, int numeroDaVenda = 0, Venda venda = null)
        {

            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            var produtoAntigo = await _produtoServices.GetProdutosByIdAsync(produtoVendido.IdDoProduto);
            var quantidade = produtoVendido.Quantidade;
            if (produtoAntigo.IsVolume)
            {
                var codigo = produtoAntigo.CodigoDoProdutoVolume;
                quantidade = produtoAntigo.QuantidadeVolume;
                produtoAntigo = await _produtoServices.GetProdutosByIdAsync((Guid)codigo);
            }
            produtoAntigo.Quantidade -= quantidade;
            produtoVendido.NumeroDaVenda = numeroDaVenda;
            produtoVendido.Venda = venda;
            produtoVendido.IdDoUsuario = userId;
            if(produtoVendido.Despesa) produtoVendido.DespesaDate = DateTime.Now;


            _context.Add(produtoVendido);
            await _context.SaveChangesAsync();
            await _produtoServices.UpdateProdutoAsync(produtoAntigo);
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
                        quantidade = produtoAntigo.QuantidadeVolume;
                        produtoAntigo = await _produtoServices.GetProdutosByIdAsync((Guid)codigo);
                    }
                    produtoAntigo.Quantidade += quantidade;
                    await _produtoServices.UpdateProdutoAsync(produtoAntigo);
                }
            }
        }

        public async Task<decimal> TotalDeDispesasPorPeriodo(DateTime? inicial, DateTime? final)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;
            var produtosVendido = await _context.ProdutosVendidos.Where(x => x.DespesaDate.Date >= inicial && x.DespesaDate.Date <= final && x.IdDoUsuario == userId).ToListAsync();
            decimal? total = 0;
            if(produtosVendido.Count > 0)
            {
                foreach (var item in produtosVendido)
                {
                    var prod = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                    if(prod != null) total += prod.Valor * item.Quantidade;
                }
            }

            return (decimal)total;
        }
    }
}
