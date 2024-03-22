using System.ComponentModel.DataAnnotations.Schema;

namespace Comandas.Data.Models
{
    public class RelatorioApuracao
    {
        public string NomeMetodo { get; set; }
        public decimal ValorApurado { get; set; }
        public decimal ValorInformado { get; set; }
        public decimal ValorDiferenca { get; set; }
    }
}
