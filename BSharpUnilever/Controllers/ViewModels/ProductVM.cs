using BSharpUnilever.Controllers.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(255)]
        public string Barcode { get; set; }

        [StringLength(255)]
        public string SapCode { get; set; }

        [StringLength(255)]
        [ChoiceList("HC", "PC", "F&R", "O")]
        public string Type { get; set; } // Home Care (HC), Personal Care (PC), Food & Refreshments (F&R), Other (O)

        public bool IsPromo { get; set; }

        public bool IsActive { get; set; }
    }
}
