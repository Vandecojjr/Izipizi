using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class HistoricoEmAberto
    {
        public Guid Id { get; set; }
        public string UserID { get; set; }
        public Guid IdEmAberto { get; set; }
        public bool IsEntrada { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }
        public DateTime Horario { get; set; }
    }
}
