using AutoMapper;
using BizOpsAPI.Models;
using BizOpsAPI.DTOs;

namespace BizOpsAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<UpdateUserDto, User>();

            CreateMap<ClientCreateDto, Client>();
            CreateMap<Client, ClientDto>();
            CreateMap<ClientUpdateDto, Client>();

            CreateMap<CreateInvoiceDto, Invoice>();
            CreateMap<CreateInvoiceItemDto, InvoiceItem>()
                .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * s.UnitPrice));

            CreateMap<Invoice, InvoiceDto>();
            CreateMap<InvoiceItem, InvoiceItemDto>();
        }
    }
}
