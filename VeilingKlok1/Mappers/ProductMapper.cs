using System.Linq.Expressions;
using VeilingKlokApp.Models;
using VeilingKlokApp.Models.Domain;
using VeilingKlokApp.Models.InputDTOs;

namespace VeilingKlokApp.Mappers
{
    public static class ProductMapper
    {
        public static Expression<Func<Product, ProductDetails>> ProjectToDetails =>
            entity => new ProductDetails
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                MinimumPrice = entity.MinimumPrice,
                Quantity = entity.Stock,
                ImageBase64 = entity.ImageBase64,
                Size = entity.Size,
            };

        public static Product ToEntity(NewProduct dto, Guid kwekerId)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                MinimumPrice = dto.MinimumPrice,
                Stock = dto.Quantity,
                ImageBase64 = dto.ImageBase64,
                Size = dto.Size,
                KwekerId = kwekerId,
            };
        }

        public static ProductDetails ToDetails(Product entity)
        {
            return new ProductDetails
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                MinimumPrice = entity.MinimumPrice,
                Quantity = entity.Stock,
                ImageBase64 = entity.ImageBase64,
                Size = entity.Size,
            };
        }

        public static void UpdateEntity(Product entity, UpdateProduct dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                entity.Name = dto.Name;
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                entity.Description = dto.Description;
            }

            if (dto.Price.HasValue)
            {
                entity.Price = dto.Price.Value;
            }

            if (dto.MinimumPrice.HasValue)
            {
                entity.MinimumPrice = dto.MinimumPrice.Value;
            }

            if (dto.Quantity.HasValue)
            {
                entity.Stock = dto.Quantity.Value;
            }

            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                entity.ImageBase64 = dto.ImageBase64;
            }

            if (!string.IsNullOrWhiteSpace(dto.Size))
            {
                entity.Size = dto.Size;
            }
        }
    }
}
