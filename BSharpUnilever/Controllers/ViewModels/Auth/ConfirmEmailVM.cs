using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels.Auth
{
    public class ConfirmEmailVM
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string EmailConfirmationToken { get; set; }
    }
}
