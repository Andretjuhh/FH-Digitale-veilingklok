using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class KwekerMapper : IBaseMapper<Kweker, KwekerOutputDto>
{
    public static Expression<Func<Kweker, KwekerOutputDto>> EntityDto =>
        entity => new KwekerOutputDto
        {
            Email = entity.Email,
            KvkNumber = entity.KvkNumber,
            CompanyName = entity.CompanyName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Telephone = entity.Telephone,
            Address = AddressMapper.ToOutputDto(entity.Adress),
        };

    public static KwekerOutputDto ToOutputDto(Kweker entity)
    {
        return new KwekerOutputDto
        {
            Email = entity.Email,
            KvkNumber = entity.KvkNumber,
            CompanyName = entity.CompanyName,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Telephone = entity.Telephone,
            Address = AddressMapper.ToOutputDto(entity.Adress),
        };
    }
}
