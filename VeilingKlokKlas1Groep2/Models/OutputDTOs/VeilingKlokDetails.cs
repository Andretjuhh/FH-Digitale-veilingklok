namespace VeilingKlokApp.Models.OutputDTOs;

/// <summary>
/// DTO for returning VeilingKlok (Auction Clock) details
/// </summary>
public class VeilingKlokDetails
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public int DurationInSeconds { get; set; }
    public int LiveViews { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int VeilingmeesterId { get; set; }
}
