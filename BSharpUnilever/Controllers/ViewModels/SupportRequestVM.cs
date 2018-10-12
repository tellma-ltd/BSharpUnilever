using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BSharpUnilever.Controllers.ViewModels
{
    public class SupportRequestVM
    {
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        public int? SerialNumber { get; set; }

        [ChoiceList(
            SupportRequestStates.Draft, SupportRequestStates.Submitted, SupportRequestStates.Approved,
            SupportRequestStates.Posted, SupportRequestStates.Canceled, SupportRequestStates.Rejected)]
        public string State { get; set; }

        [Required]
        public UserVM AccountExecutive { get; set; }

        [Required]
        public UserVM Manager { get; set; }

        [Required]
        [ChoiceList(Reasons.DisplayContract, Reasons.PremiumSupport, Reasons.PriceReduction, Reasons.FromBalance)]
        public string Reason { get; set; }

        [Required]
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

        public int ToState { get; set; } // Same states as SupportRequest

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

        public DateTime Date { get; set; }
    }
}
