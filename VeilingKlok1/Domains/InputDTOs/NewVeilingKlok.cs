using System.ComponentModel.DataAnnotations;

namespace VeilingKlokApp.Models.InputDTOs;

/// <summary>
/// DTO for creating a new VeilingKlok (Auction Clock)
/// </summary>
public class NewVeilingKlok
{
    [Required]
    public required string Naam { get; set; }

    [Required]
    public int DurationInSeconds { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public Guid VeilingmeesterId { get; set; }
}
