using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class ProdutosEmAberto
    {
        public Guid Id { get; set; }
        public Guid IdDoProduto { get; set; }
        public string IdDoUsuario { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public int NumeroComanda { get; set; }
        public string Vendedor { get; set; }
        public DateTime DataDaVenda {  get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? total { get; set; }
    }
}
