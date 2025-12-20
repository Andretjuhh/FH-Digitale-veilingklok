using System.Linq.Expressions;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokKlas1Groep2.Models.OutputDTOs;

namespace VeilingKlokApp.Mappers
{
    public static class KoperMapper
    {
        public static Expression<Func<Koper, KoperDetails>> ProjectToDetails =>
            entity => new KoperDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Adress = entity.Adress,
                Regio = entity.Regio,
            };

        public static AccountDetails ToAccountDetails(Koper entity)
        {
            return new AccountDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Regio = entity.Regio,
                Adress = entity.Adress,
                PostCode = entity.PostCode,
                AccountType = AccountType.Koper,
            };
        }

        public static Koper ToEntity(NewKoperAccount dto, IPasswordHasher passwordHasher)
        {
            return new Koper
            {
                Email = dto.Email,
                Password = passwordHasher.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Telephone = dto.Telephone,
                Adress = dto.Adress,
                PostCode = dto.PostCode,
                Regio = dto.Regio,
            };
        }

        public static void UpdateEntity(
            Koper entity,
            UpdateKoperProfile dto,
            IPasswordHasher? passwordHasher = null
        )
        {
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                entity.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password) && passwordHasher != null)
            {
                entity.Password = passwordHasher.HashPassword(dto.Password);
            }

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
            {
                entity.FirstName = dto.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(dto.LastName))
            {
                entity.LastName = dto.LastName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Adress))
            {
                entity.Adress = dto.Adress;
            }

            if (!string.IsNullOrWhiteSpace(dto.PostCode))
            {
                entity.PostCode = dto.PostCode;
            }

            if (!string.IsNullOrWhiteSpace(dto.Regio))
            {
                entity.Regio = dto.Regio;
            }
        }
    }
}
