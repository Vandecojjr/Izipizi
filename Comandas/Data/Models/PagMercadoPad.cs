using System;
using System.ComponentModel.DataAnnotations;

namespace Comandas.Data.Models
{
    public class PagMercadoPad
    {
        [Key]
        public Guid Id { get; set; }
        public string? IdCompra { get; set; }
        public string? EmailUser { get; set; }
    }
}

