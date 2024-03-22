using Comandas.Data;
using Comandas.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Comandas.Services
{
    public class FormaDePagamentoServices : IFormaDePagamentoServices
    {
        private readonly IMetodoDePagamentoServices _metodoDePagamentoServices;
        private readonly ApplicationDbContext _context;
        private readonly ITransacaoServices _transacaoServices;

        public FormaDePagamentoServices(IMetodoDePagamentoServices metodoDePagamentoServices, ApplicationDbContext context, ITransacaoServices transacaoServices)
        {
            _metodoDePagamentoServices = metodoDePagamentoServices;
            _context = context;
            _transacaoServices = transacaoServices;
        }

        public async Task AddFormaDePAgamento(FormaDePagamento formaDePagamento, Venda venda)
        {
            var metodo = await _metodoDePagamentoServices.GetMetodoDePagamentoAsync(formaDePagamento.MetodoDePagamentoId);
            formaDePagamento.NomeDoMetodo = metodo.Nome;
            formaDePagamento.Venda = venda;

            _context.Add(formaDePagamento);
            await _context.SaveChangesAsync();

            Transacao transacao = new();
            transacao.Nome = $"Venda N° {venda.Numero}";
            transacao.MetodoId = metodo.Id;
            transacao.Valor = formaDePagamento.Valor;
            transacao.Tipo = true;

            await _transacaoServices.AddTransacaoAsync(transacao);
        }

        public async Task<List<FormaDePagamento>> GetFormaDePagamentos(Venda venda)
        {
            var formasDePagamento = await _context.FormaDePagamento.Where(x => x.Venda == venda).ToListAsync();
            return formasDePagamento;
        }
    }
}
