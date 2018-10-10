using BSharpUnilever.Controllers.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class SupportRequestVM
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int SerialNumber { get; set; }

        [ChoiceList(0, 10, 20, 100, -1, -10)]
        public int State { get; set; } // Draft (0), Submitted (10), Approved (20), Posted (100), Rejected (-1), Canceled (-10)

        public string AccountExecutiveId { get; set; }
        public UserVM AccountExecutive { get; set; }

        [Required]
        public string ManagerId { get; set; }
        public UserVM Manager { get; set; }

        [Required]
        [ChoiceList("DC", "PS", "PR", "FB")]
        public string Reason { get; set; } // Display Contract (DC), Premium Support (PS), Price Reduction (PR), From Balance (FB)

        public int StoreId { get; set; }
        public StoreVM Store { get; set; }

        [StringLength(1023)]
        public string Comment { get; set; }

        public List<SupportRequestLineItemVM> LineItems { get; set; }

        public List<StateChangeVM> StateChanges { get; set; }

        public List<GeneratedDocumentVM> GeneratedDocuments { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? Created { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? Modified { get; set; }
    }

    // This is a weak VM, keep in the same file
    public class SupportRequestLineItemVM
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }
        public ProductVM Product { get; set; }

        public decimal Quantity { get; set; }

        public decimal RequestedSupport { get; set; }

        public decimal RequestedValue { get; set; }

        public decimal ApprovedSupport { get; set; }

        public decimal ApprovedValue { get; set; }

        public decimal UsedSupport { get; set; }

        public decimal UsedValue { get; set; }
    }

    // Another weak VM, keep in the same file
    // This is always generated server side, no need for validation attributes
    public class StateChangeVM
    {
        public int Id { get; set; }

        public int FromState { get; set; } // Same states as SupportRequest

        public int ToState { get; set; }// Same states as SupportRequest

        public DateTimeOffset Time { get; set; }

        public string UserId { get; set; }
        public UserVM User { get; set; }
    }

    // Another weak VM, keep in the same file
    // This is always generated server side, no need for validation attribute
    public class GeneratedDocumentVM
    {
        public int Id { get; set; }

        public int SerialNumber { get; set; }

        public int State { get; set; } // Valid (0), Void (-1)
    }
}
