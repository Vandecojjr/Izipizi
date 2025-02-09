﻿using Comandas.Data.Models;

namespace Comandas.Services
{
    public interface ITransacaoServices
    {
        Task<bool> AddTransacaoAsync(Transacao transacao);
        Task<List<Transacao>> GetAllTrasacoesAsync(Caixa caixa = null);
        Task<List<Transacao>> GetTransacoesByMetodoAsync(Guid metodoId , Caixa caixa);
    }
}
