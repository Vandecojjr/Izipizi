using Comandas.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int nivelAdmin {  get; set; } // 0 (easy), 1 (proprietario), 2 (funcionario).
        public string? IdDoProprietario { get; set; } //Caso o usuario seja funcionario tera que conter o id Do usuario Proprietario.
        public string? NomeEmpresa { get; set; } // Nome da empresa pertencente.
        public int TotalDeUsuarios { get; set; }
        public bool IsAtivo { get; set; }
        public bool IsLogados { get; set; }
        public string? Estado { get; set; }
        public DateTime DataDeAdesao { get; set; }
        public DateTime? DataDeBloqueio { get; set; }
        public string? estadoDePagamento { get; set; }
        public int DiaDeVencimento { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? porcentagemDefinida { get; set; }
        public ICollection<Produto>? Produtos { get; set; }
        public ICollection<Categoria>? Categorias { get; set; }
    }

}
