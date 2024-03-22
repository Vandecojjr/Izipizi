using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comandas.Data.Models
{
    public class Produto
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }
        public int? Quantidade { get; set; }
        public bool IsActive { get; set; }
        public bool IsControled { get; set; }

        public bool IsVolume {  get; set; }
        public Guid? CodigoDoProdutoVolume { get; set; }
        public int? QuantidadeVolume { get; set; }

        //propriedades relacionadas a precificaçao e custo do produto;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Valor { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ValorDeCusto { get; set; }
        public double? MargemLucro { get; set; }
        public bool PrecoAutomatico { get; set; }

        //Propriedades relacionadas ao usuario do produto
        public string? ApplicationUserId { get; set; }
        public virtual ApplicationUser? UserId { get; set; }

        //Propriedades relacionadas a categoria do produto
        public string NomeDaCategoria { get; set; }

        [Required(ErrorMessage = "Uma categoria deve ser selecionada")]
        public Guid? ID_categoria { get; set; }
        public virtual Categoria? Categoria { get; set; }


    }
}
