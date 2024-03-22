using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string IdUsuario { get; set; }
        public string Nome { get; set; }
        public string? Rua { get; set; }
        public int? Numero { get; set; }
        public string? Estado { get; set; }
        public string? Telefone { get; set; }    
        public string? CPF { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Limite { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? LimiteRestante { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? LimiteAntigo{ get; set; }
    }
}
