namespace Comandas.Data.Models
{
    public class RelatorioDeTransacoesCaixa
    {
        public string Nome { get; set; }
        public string UserNome { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public string MetodoNome { get; set; }
    }
}
