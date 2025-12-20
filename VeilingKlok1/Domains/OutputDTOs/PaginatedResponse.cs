namespace VeilingKlokApp.Models.OutputDTOs
{
    /// <summary>
    /// Generic paginated response wrapper
    /// </summary>
    public class PaginatedResponse<T>
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalCount { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
