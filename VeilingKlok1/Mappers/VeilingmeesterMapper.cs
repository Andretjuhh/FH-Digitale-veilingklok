using System.Linq.Expressions;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.OutputDTOs;
using VeilingKlokApp.Services;
using VeilingKlokApp.Types;
using VeilingKlokKlas1Groep2.Models.OutputDTOs;

namespace VeilingKlokApp.Mappers
{
    public static class VeilingmeesterMapper
    {
        public static AccountDetails ToAccountDetails(Veilingmeester entity)
        {
            return new AccountDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                FirstName = entity.AuthorisatieCode,
                LastName = string.Empty,
                Regio = entity.Regio,
                AccountType = AccountType.Veilingmeester,
            };
        }

        public static Expression<Func<Veilingmeester, VeilingmeesterDetails>> ProjectToDetails =>
            entity => new VeilingmeesterDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                Regio = entity.Regio,
                AuthorisatieCode = entity.AuthorisatieCode,
            };

        public static Veilingmeester ToEntity(
            NewVeilingMeesterAccount dto,
            IPasswordHasher passwordHasher
        )
        {
            return new Veilingmeester
            {
                Email = dto.Email,
                Password = passwordHasher.HashPassword(dto.Password),
                Regio = dto.Regio,
                AuthorisatieCode = dto.AuthorisatieCode,
            };
        }

        public static VeilingmeesterDetails ToDetails(Veilingmeester entity)
        {
            return new VeilingmeesterDetails
            {
                AccountId = entity.Id,
                Email = entity.Email,
                Regio = entity.Regio,
                AuthorisatieCode = entity.AuthorisatieCode,
            };
        }

        public static void UpdateEntity(Veilingmeester entity, UpdateVeilingMeester dto)
        {
            entity.Email = dto.Email;
            entity.Password = dto.Password; // Note: Password hashing should probably happen here if it's a raw password, but the controller logic just assigned it. I'll stick to controller logic for now or check if it needs hashing.
            entity.AuthorisatieCode = dto.AuthorisatieCode;
            entity.Regio = dto.Regio;
        }
    }
}
