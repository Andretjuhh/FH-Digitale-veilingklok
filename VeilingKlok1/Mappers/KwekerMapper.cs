using System.Linq.Expressions;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokKlas1Groep2.Models.OutputDTOs;

namespace VeilingKlokApp.Mappers
{
    public static class KwekerMapper
    {
        public static AccountDetails ToAccountDetails(Kweker entity)
        {
            return new AccountDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                FirstName = entity.Name,
                LastName = string.Empty,
                CompanyName = entity.Name,
                PhoneNumber = entity.Telephone,
                Adress = entity.Adress,
                Regio = entity.Regio,
                KvkNumber = entity.KvkNumber,
                AccountType = AccountType.Kweker,
            };
        }

        public static Expression<Func<Kweker, KwekerDetails>> ProjectToDetails =>
            entity => new KwekerDetails
            {
                AccountId = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Telephone = entity.Telephone,
                Adress = entity.Adress,
                Regio = entity.Regio,
                KvkNumber = entity.KvkNumber,
                PostCode = entity.PostCode,
            };

        public static Kweker ToEntity(NewKwekerAccount dto, IPasswordHasher passwordHasher)
        {
            return new Kweker
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Password = passwordHasher.HashPassword(dto.Password),
                Name = dto.Name,
                Telephone = dto.Telephone,
                Adress = dto.Adress,
                Regio = dto.Regio,
                KvkNumber = dto.KvkNumber,
                PostCode = dto.PostCode,
            };
        }
    }
}
