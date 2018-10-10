using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Data.Entities;
using System.ComponentModel.DataAnnotations;

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
        [ChoiceList(Roles.KAE, Roles.Manager, Roles.Administrator, Roles.Inactive)]
        public string Role { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
