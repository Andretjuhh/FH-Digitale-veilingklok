using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class CreateOrderDTO
{
    [Required] public Guid VeilingKlokId { get; set; }
    [Required] public Guid ProductItemId { get; set; }
    [Required] [Range(1, int.MaxValue)] public int Quantity { get; init; }
}