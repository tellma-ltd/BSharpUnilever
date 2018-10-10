using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BSharpUnilever.Data.Entities
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Role { get; set; }

        public ICollection<Store> AssignedStores { get; set; }

        public ICollection<SupportRequest> Requests { get; set; }

        public ICollection<SupportRequest> ManagedRequests { get; set; }
    }

    public static class Roles
    {
        public const string KAE = "KAE";
        public const string Manager = "Manager";
        public const string Administrator = "Administrator";
        public const string Inactive = "Inactive";
    }
}