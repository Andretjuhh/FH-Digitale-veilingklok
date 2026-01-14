namespace Application.DTOs.Output;

public class OverallAveragePriceOutputDto
{
    public required decimal AveragePrice { get; set; }
    public required int SampleCount { get; set; }
}
