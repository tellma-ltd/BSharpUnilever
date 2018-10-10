using AutoMapper;
using BSharpUnilever.Controllers.ViewModels;
using BSharpUnilever.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers.Mapper
{
    /// <summary>
    /// Defines the AutoMapper mappings
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
        }
    }
}
