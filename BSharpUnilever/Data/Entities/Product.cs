using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Barcode { get; set; }

        [MaxLength(255)]
        public string SapCode { get; set; }

        [MaxLength(255)]
        public string Type { get; set; } // Home Care (HC), Food Care (FC), Food & Refreshments (F&R), Other (O)

        public bool IsPromo { get; set; }

        public ICollection<SupportRequestLineItem> SupportRequestLineItems { get; set; }
    }
}
