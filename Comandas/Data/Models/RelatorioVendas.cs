using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comandas.Data.Models
{
    public class RelatorioVendas
    {
        public int? Numero { get; set; }
        public decimal? Total { get; set; }

        public decimal? Descontos { get; set; }
        public string? NomeDoUsuario { get; set; }
        public string? NomeDoCliente { get; set; }
        public decimal? Lucro { get; set; }
        public decimal? TotalCliente { get; set; }
        public DateTime? Data { get; set; }
    }
}
