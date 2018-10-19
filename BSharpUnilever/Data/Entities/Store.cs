using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Data.Entities
{
    public class Store
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string AccountExecutiveId { get; set; }

        public User AccountExecutive { get; set; }

        public bool IsActive { get; set; }

        public ICollection<SupportRequest> SupportRequests { get; set; }
    }
}
