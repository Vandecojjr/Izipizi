using System.ComponentModel.DataAnnotations;

namespace Comandas.Data.Models
{
    public class Categoria
    {
        public Guid CategoriaId { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }
        public string? Descricao { get; set; }

        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? UserId { get; set; }

        public ICollection<Produto>? Produtos { get; set; }
    }
}
