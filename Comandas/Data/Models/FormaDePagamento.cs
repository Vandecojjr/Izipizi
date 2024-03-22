using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class FormaDePagamento
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Selecione uma forma de pagamento")]
        public Guid MetodoDePagamentoId { get; set; }
        public string NomeDoMetodo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }
        public Guid VendaId { get; set; }
        public virtual Venda? Venda { get; set; }
    }
}
