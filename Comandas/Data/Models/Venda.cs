using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class Venda
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DataDaVenda { get; set; }
        public int Numero { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Descontos { get; set; }
        public string IdUsuario { get; set; }
        public string NomeDoUsuario { get; set; }
        public string? NomeDoCliente { get; set; }
        public bool IsPrazo { get; set; }
        public bool IsPago { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Lucro { get; set; }
        public Guid? IdCliente { get; set; }
        public ICollection<ProdutoVendido> produtoVendidos { get; set; }
        public ICollection<FormaDePagamento> formasDePagamentosDaVenda { get; set; }

        public virtual Caixa? Caixa { get; set; }
    }
}

