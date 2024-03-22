namespace Comandas.Data.Models
{
    public class MetodoDePagamento
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string IdDoUsuario { get; set; }

        public bool Padrao { get; set; }
    }
}
