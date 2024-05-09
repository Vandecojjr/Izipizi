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

        public async Task<bool> AddFormaDePAgamento(FormaDePagamento? formaDePagamento = null, Venda? venda = null, List<FormaDePagamento>? formas = null)
        {
            try
            {

                if (formas == null)
                {
                    formas = new();
                    formas.Add(formaDePagamento);
                }
                foreach (var item in formas)
                {
                    var metodo = await _metodoDePagamentoServices.GetMetodoDePagamentoAsync(item.MetodoDePagamentoId);
                    item.NomeDoMetodo = metodo.Nome;
                    item.Venda = venda;

                    Transacao transacao = new();
                    transacao.Nome = $"Venda N° {venda.Numero}";
                    transacao.MetodoId = metodo.Id;
                    transacao.Valor = item.Valor == null ? 0 : (decimal)item.Valor;
                    transacao.Tipo = true;

                    await _transacaoServices.AddTransacaoAsync(transacao);
                }

                _context.AddRange(formas);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<List<FormaDePagamento>> GetFormaDePagamentos(Venda venda)
        {
            var formasDePagamento = await _context.FormaDePagamento.Where(x => x.Venda == venda).ToListAsync();
            return formasDePagamento;
        }
    }
}
