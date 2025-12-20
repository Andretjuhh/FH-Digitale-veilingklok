using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

public class CreateOrderDTO
{
    [Required] public Guid KoperId { get; set; }
}