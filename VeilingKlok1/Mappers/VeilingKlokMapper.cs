using System.Linq.Expressions;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.InputDTOs;
using VeilingKlokApp.Models.OutputDTOs;

namespace VeilingKlokApp.Mappers
{
    public static class VeilingKlokMapper
    {
        public static Expression<Func<VeilingKlok, VeilingKlokDetails>> ProjectToDetails =>
            entity => new VeilingKlokDetails
            {
                Id = entity.Id,
                Naam = entity.Naam,
                DurationInSeconds = entity.DurationInSeconds,
                LiveViews = entity.LiveViews,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                VeilingmeesterId = entity.VeilingmeesterId,
            };

        public static VeilingKlok ToEntity(NewVeilingKlok dto)
        {
            return new VeilingKlok
            {
                Naam = dto.Naam,
                DurationInSeconds = dto.DurationInSeconds,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                VeilingmeesterId = dto.VeilingmeesterId,
                LiveViews = 0,
            };
        }

        public static VeilingKlokDetails ToDetails(VeilingKlok entity)
        {
            return new VeilingKlokDetails
            {
                Id = entity.Id,
                Naam = entity.Naam,
                DurationInSeconds = entity.DurationInSeconds,
                LiveViews = entity.LiveViews,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                VeilingmeesterId = entity.VeilingmeesterId,
            };
        }

        public static void UpdateEntity(VeilingKlok entity, UpdateVeilingKlok dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Naam))
            {
                entity.Naam = dto.Naam;
            }

            if (dto.DurationInSeconds.HasValue)
            {
                entity.DurationInSeconds = dto.DurationInSeconds.Value;
            }

            if (dto.StartTime.HasValue)
            {
                entity.StartTime = dto.StartTime.Value;
            }

            if (dto.EndTime.HasValue)
            {
                entity.EndTime = dto.EndTime.Value;
            }

            if (dto.LiveViews.HasValue)
            {
                entity.LiveViews = dto.LiveViews.Value;
            }
        }
    }
}
