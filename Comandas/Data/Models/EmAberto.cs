using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class EmAberto
    {
        public Guid Id { get; set; }
        public string IdDoUsuario { get; set; }
        public string NomeVendedor { get; set; }
        public string Nome { get; set; }
        public DateTime DataDeAbertura { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total {  get; set; }
        

    }
}
