namespace Application.DTOs.Output;

public class KwekerAveragePriceOutputDto
{
    public required Guid KwekerId { get; set; }
    public required string KwekerName { get; set; }
    public required decimal AveragePrice { get; set; }
    public required int SampleCount { get; set; }
}
