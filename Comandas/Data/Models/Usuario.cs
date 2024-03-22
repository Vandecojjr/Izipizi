namespace Comandas.Data.Models
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string? Senha { get; set; }
        public string? Empresa { get; set; }
        public int NivelAdmin { get; set; }
        public bool Ativo { get; set; }
        public bool Logado { get; set; }
        public DateTime DataDaAdesao { get; set; } 
        public int DiaDeVencimento { get; set; }    
        public decimal? porcentagemUsuario { get; set; }
        public int UsuariosAltorizados { get; set; }
        public List<PerfilUsuario> PerfisDesteUser { get; set; } = new List<PerfilUsuario>();
    }
}
