using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class AddressMapper : IBaseMapper<Address, AddressOutputDto>
{
    public static Expression<Func<Address, AddressOutputDto>> EntityDto =>
        entity => new AddressOutputDto
        {
            Id = entity.Id,
            Street = entity.Street,
            City = entity.City,
            RegionOrState = entity.RegionOrState,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
        };

    public static AddressOutputDto ToOutputDto(Address entity)
    {
        return new AddressOutputDto
        {
            Id = entity.Id,
            Street = entity.Street,
            City = entity.City,
            RegionOrState = entity.RegionOrState,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
        };
    }
}
