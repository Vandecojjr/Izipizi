using Comandas.Data;
using Comandas.Data.Models;
using Comandas.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Comandas.Services
{
    public class ProdutoVendidoServices : IProdutoVendidoServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IProdutoServices _produtoServices;

        public ProdutoVendidoServices(ApplicationDbContext context, IProdutoServices produtoServices)
        {
            _context = context;
            _produtoServices = produtoServices;
        }

        public async Task AddProdutoVendidoAsync(ProdutoVendido produtoVendido, int numeroDaVenda, Venda venda)
        {
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
    }
}
