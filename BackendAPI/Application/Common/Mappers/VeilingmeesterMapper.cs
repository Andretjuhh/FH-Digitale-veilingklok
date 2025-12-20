using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.DTOs.Output;
using Domain.Entities;

namespace Application.Common.Mappers;

public class VeilingmeesterMapper : IBaseMapper<Veilingmeester, VeilingmeesterOutputDto>
{
    public static Expression<Func<Veilingmeester, VeilingmeesterOutputDto>> EntityDto =>
        entity => new VeilingmeesterOutputDto
        {
            Email = entity.Email,
            CountryCode = entity.CountryCode,
            Region = entity.Region,
        };

    public static VeilingmeesterOutputDto ToOutputDto(Veilingmeester entity)
    {
        return new VeilingmeesterOutputDto
        {
            Email = entity.Email,
            CountryCode = entity.CountryCode,
            Region = entity.Region,
        };
    }
}
