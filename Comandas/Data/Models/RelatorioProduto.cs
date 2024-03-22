namespace Comandas.Data.Models
{
    public class RelatorioProduto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public int? Quantidade { get; set; }
        public decimal? Valor { get; set; }
        public decimal? ValorDeCusto { get; set; }
        public double? MargemLucro { get; set; }
        public string NomeDaCategoria { get; set; }
    }
}
