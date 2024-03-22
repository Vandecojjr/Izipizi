using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class FechamentoCaixa
    {
        public Guid Id { get; set; }
        public string NomeMetodo { get; set; }
        public Guid IdMetodo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorApurado { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal ValorInformado { get; set; }
        public virtual Caixa? Caixa { get; set; }
    }
}
