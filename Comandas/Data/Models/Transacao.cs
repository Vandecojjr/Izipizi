using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class Transacao
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserNome { get; set; }

        [Required(ErrorMessage = "O nome é obrigatorio")]
        public string Nome { get; set;}
        public DateTime Data {  get; set; }

        // se for entrada  = true se for saida = false
        public bool Tipo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O metodo de transação obrigatorio")]
        public Guid MetodoId { get; set; }
        public string MetodoNome { get; set; }
        public virtual Caixa? Caixa { get; set; }

    }
}
