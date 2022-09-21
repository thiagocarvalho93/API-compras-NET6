using ApiDotnet.Application.DTOs;
using ApiDotnet.Domain.Entities;
using AutoMapper;

namespace ApiDotnet.Application.Mappings
{
    public class DTOToDomainMapping : Profile
    {
        public DTOToDomainMapping()
        {
            CreateMap<PersonDTO, Person>();
            CreateMap<ProductDTO, Product>();
        }
    }
}