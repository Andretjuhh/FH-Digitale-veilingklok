using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Models;
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
            PrimaryAddressId = entity.PrimaryAdressId ?? 0,
            Addresses = entity.Adresses.AsQueryable().Select(AddressMapper.EntityDto).ToList()
        };

    public static KoperOutputDto ToOutputDto(Koper entity)
    {
        return new KoperOutputDto
        {
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Telephone = entity.Telephone,
            PrimaryAddressId = entity.PrimaryAdressId ?? 0,
            Addresses = entity.Adresses.Select(AddressMapper.ToOutputDto).ToList()
        };
    }

    public class Info : IBaseMapper<KoperInfo, KoperInfoOutputDto>
    {
        public static Expression<Func<KoperInfo, KoperInfoOutputDto>> EntityDto =>
            entity => new KoperInfoOutputDto
            {
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Telephone = entity.Telephone,
                Address = AddressMapper.EntityDto.Compile()(entity.Address)
            };

        public static KoperInfoOutputDto ToOutputDto(KoperInfo entity)
        {
            return new KoperInfoOutputDto
            {
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Telephone = entity.Telephone,
                Address = AddressMapper.ToOutputDto(entity.Address)
            };
        }
    }
}