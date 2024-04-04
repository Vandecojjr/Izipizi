using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class EmAberto
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public string IdDoUsuario { get; set; }
        public string NomeVendedor { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        public string Nome { get; set; }
        public DateTime DataDeAbertura { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Total {  get; set; }
        

    }
}
