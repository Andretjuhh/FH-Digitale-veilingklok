using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Input;

/// <summary>
/// DTO for creating a new VeilingKlok (Auction Clock)
/// </summary>
public class CreateVeilingKlokDTO
{
    [Required(ErrorMessage = "VEILINGKLOK.SCHEDULED_TIME_REQUIRED")]
    public DateTimeOffset ScheduledAt { get; set; }

    [Required(ErrorMessage = "VEILINGKLOK.DURATION_REQUIRED")]
    [Range(1, 60, ErrorMessage = "VEILINGKLOK.DURATION_INVALID")]
    public int VeilingDurationMinutes { get; set; }

    [Required(ErrorMessage = "VEILINGKLOK.PRODUCTS_REQUIRED")]

    // ProductId mapped with price
    [MinLength(1, ErrorMessage = "VEILINGKLOK.AT_LEAST_ONE_PRODUCT_REQUIRED")]
    public Dictionary<Guid, decimal> Products { get; set; }
}