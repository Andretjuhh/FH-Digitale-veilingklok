using System.ComponentModel.DataAnnotations;
using Qowaiv.Validation.DataAnnotations;

namespace Application.DTOs.Input;

/// <summary>
/// DTO for creating a new VeilingKlok (Auction Clock)
/// </summary>
public class CreateVeilingKlokDTO
{
    [Required(ErrorMessage = "VEILINGKLOK.SCHEDULED_TIME_REQUIRED")]
    [InFuture(ErrorMessage = "VEILINGKLOK.SCHEDULED_TIME_NOT_IN_FUTURE")]
    public DateTimeOffset ScheduledAt { get; set; }

    [Required(ErrorMessage = "VEILINGKLOK.DURATION_REQUIRED")]
    [Range(10, 1000, ErrorMessage = "VEILINGKLOK.DURATION_INVALID")]
    public int VeilingDurationSeconds { get; set; }

    // [Required(ErrorMessage = "VEILINGKLOK.PRODUCTS_REQUIRED")]
    // [MinLength(1, ErrorMessage = "VEILINGKLOK.AT_LEAST_ONE_PRODUCT_REQUIRED")]
    public Dictionary<Guid, decimal> Products { get; set; } = new();
}