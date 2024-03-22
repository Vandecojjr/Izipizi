using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class LogPrecoProduto
    {
        public Guid Id { get; set; }
        public string IdUsuario { get; set; }
        public Guid IdProduto { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Custo { get; set; }
        public string Tag { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Lucro { get; set; }
        public string? Obs { get; set; }
        public DateTime Data { get; set; }

    }
}
