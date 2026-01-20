using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class AdminMapper : IBaseMapper<Account, AdminOutputDto>
{
    public static Expression<Func<Account, AdminOutputDto>> EntityDto =>
        entity => new AdminOutputDto { Email = entity.Email ?? string.Empty };

    public static AdminOutputDto ToOutputDto(Account entity)
    {
        return new AdminOutputDto { Email = entity.Email ?? string.Empty };
    }
}
