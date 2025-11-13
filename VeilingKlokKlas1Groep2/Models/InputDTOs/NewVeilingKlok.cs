namespace VeilingKlokKlas1Groep2.Models.InputDTOs;

/// <summary>
/// DTO for creating a new VeilingKlok (Auction Clock)
/// </summary>
public class NewVeilingKlok
{
    public string Naam { get; set; } = string.Empty;
    public int DurationInSeconds { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int VeilingmeesterId { get; set; }
}
