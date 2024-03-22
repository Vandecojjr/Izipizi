using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comandas.Data.Models
{
    public class ProdutoVendidoRelatorio
    {
        public string Nome { get; set; }
        public int? Quantidade { get; set; }
        public decimal? Desconto { get; set; }
        public decimal? Lucro { get; set; }
        public decimal? Total { get; set; }
    }
}
