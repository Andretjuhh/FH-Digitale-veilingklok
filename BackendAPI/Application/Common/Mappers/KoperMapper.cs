using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class KoperMapper : IBaseMapper<Koper, KoperOutputDto>
{
    public static Expression<Func<Koper, KoperOutputDto>> EntityDto =>
        entity => new KoperOutputDto
        {
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Telephone = entity.Telephone,
            PrimaryAddressId = entity.PrimaryAdressId,
            Addresses = entity.Adresses.AsQueryable().Select(AddressMapper.EntityDto).ToList(),
        };

    public static KoperOutputDto ToOutputDto(Koper entity)
    {
        return new KoperOutputDto
        {
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Telephone = entity.Telephone,
            PrimaryAddressId = entity.PrimaryAdressId,
            Addresses = entity.Adresses.Select(AddressMapper.ToOutputDto).ToList(),
        };
    }
}
