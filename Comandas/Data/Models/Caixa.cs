using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class Caixa
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; }
        public DateTime DataDeAbertura { get; set; }
        public DateTime? DataDeFechamento { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total {  get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalDeEntrada { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalDeSaida { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Abertura { get; set; }
        public bool Estado {  get; set; }

        public ICollection<Transacao>? transacoes { get; set; }
        public ICollection<FechamentoCaixa>? fhecamentos { get; set; }

        public ICollection<Venda>? Vendas { get; set; }
    }
}
