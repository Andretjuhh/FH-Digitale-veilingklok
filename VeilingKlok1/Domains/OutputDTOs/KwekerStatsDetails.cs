namespace VeilingKlokApp.Models.OutputDTOs
{
    /// <summary>
    /// DTO for Kweker statistics
    /// </summary>
    public class KwekerStatsDetails
    {
        public int TotalProducts { get; set; }
        public int ActiveAuctions { get; set; }
        public decimal TotalRevenue { get; set; }
        public int StemsSold { get; set; }
    }
}
