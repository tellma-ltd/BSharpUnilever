using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Role { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
