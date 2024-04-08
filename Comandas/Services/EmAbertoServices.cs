﻿
using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task AddProdutoEmAberto(List<ProdutoVendido> produtos, int comanda, string vendedor)
        {
            string userId = await _user.GetCurrentUserIdAsync();
            var userCurrent = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userCurrent.nivelAdmin == 2) userId = userCurrent.IdDoProprietario;

            ProdutosEmAberto produto = new();
            decimal? total = 0;
            var vendaEmAberto = await _context.VendasEmAberto.FirstOrDefaultAsync(x => x.Numero == comanda && x.IdDoUsuario == userId);
            foreach (var item in produtos)
            {
                var ajustarEstoque = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == item.IdDoProduto);
                total += ajustarEstoque.Valor * item.Quantidade;
                ajustarEstoque.Quantidade -= item.Quantidade;
               

                if (vendedor == null) produto.Vendedor = userCurrent.Email;
                else produto.Vendedor = vendedor;
                produto.IdDoProduto = item.IdDoProduto;
                produto.NomeProduto = item.Nome;
                produto.NumeroComanda = comanda;
                produto.Quantidade = (int)item.Quantidade;
                produto.IdDoUsuario = userId;
                produto.DataDaVenda = DateTime.Now;

                _context.Add(produto);
                await _produtoServices.UpdateProdutoAsync(ajustarEstoque, true);
            }
            vendaEmAberto.Total += (decimal)total;
            _context.Entry(vendaEmAberto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
    }
}
