using AutoMapper;
using BSharpUnilever.Controllers.ViewModels;
using BSharpUnilever.Data.Entities;

namespace BSharpUnilever.Controllers.Mapper
{
    /// <summary>
    /// Defines the property mappings for AutoMapper
    /// </summary>
    public class BSharpProfile : Profile
    {
        public BSharpProfile()
        {
            CreateMap<User, UserVM>();

            CreateMap<Store, StoreVM>();
            CreateMap<StoreVM, Store>()
                .ForMember(e => e.SupportRequests, opt => opt.Ignore())
                .ForMember(e => e.AccountExecutive, opt => opt.Ignore())
                .ForMember(e => e.AccountExecutiveId, opt => opt.MapFrom(e => e.AccountExecutive == null ? null : e.AccountExecutive.Id));

            CreateMap<Product, ProductVM>();
            CreateMap<ProductVM, Product>()
                .ForMember(e => e.SupportRequestLineItems, opt => opt.Ignore());

            CreateMap<SupportRequest, SupportRequestVM>();
            CreateMap<SupportRequestVM, SupportRequest>()
                // Navigation properties
                .ForMember(e => e.AccountExecutive, opt => opt.Ignore())
                .ForMember(e => e.AccountExecutiveId, opt => opt.MapFrom(e => e.AccountExecutive == null ? null : e.AccountExecutive.Id))
                .ForMember(e => e.Manager, opt => opt.Ignore())
                .ForMember(e => e.ManagerId, opt => opt.MapFrom(e => e.Manager == null ? null : e.Manager.Id))
                .ForMember(e => e.Store, opt => opt.Ignore())
                .ForMember(e => e.StoreId, opt => opt.MapFrom(e => e.Store == null ? 0 : e.Store.Id))

                // Navigation collections
                .ForMember(e => e.LineItems, opt => opt.Ignore())
                .ForMember(e => e.StateChanges, opt => opt.Ignore())
                .ForMember(e => e.GeneratedDocuments, opt => opt.Ignore());

            CreateMap<SupportRequestLineItem, SupportRequestLineItemVM>();
            CreateMap<SupportRequestLineItemVM, SupportRequestLineItem>()
                .ForMember(e => e.Product, opt => opt.Ignore())
                .ForMember(e => e.ProductId, opt => opt.MapFrom(e => e.Product == null ? (int?)null : e.Product.Id));

            // Note: Using the mapper profile like that to handle navigation properties is fine for this
            // small project, but we have a more robust technique in mind for projects of a larger scale
        }
    }
}
