using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class ProdutoVendido
    {
        [Key]
        public Guid Id { get; set; }
        public Guid IdDoProduto { get; set; }
        public int NumeroDaVenda { get; set; }
        public string Nome { get; set; }
        public int? Quantidade { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Desconto { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? valor { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Lucro { get; set; }
        public Guid? VendaId { get; set; }
        public virtual Venda? Venda { get; set; }

    }

}
