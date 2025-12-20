using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs;

/// <summary>
/// DTO for updating an existing VeilingKlok (Auction Clock)
/// </summary>
public class UpdateVeilingKlok
{
    public string? Naam { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than zero")]
    public int? DurationInSeconds { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Live views cannot be negative")]
    public int? LiveViews { get; set; }
}
