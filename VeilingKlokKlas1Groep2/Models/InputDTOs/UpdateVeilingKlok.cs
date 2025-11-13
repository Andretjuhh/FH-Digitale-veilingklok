namespace VeilingKlokKlas1Groep2.Models.InputDTOs;

/// <summary>
/// DTO for updating an existing VeilingKlok (Auction Clock)
/// </summary>
public class UpdateVeilingKlok
{
    public string? Naam { get; set; }
    public int? DurationInSeconds { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? LiveViews { get; set; }
}
