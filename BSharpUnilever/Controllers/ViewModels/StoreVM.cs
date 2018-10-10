using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class StoreVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public UserVM AccountExecutive { get; set; }

        public bool IsActive { get; set; }


        public List<SupportRequestVM> SupportRequests { get; set; }
    }
}
