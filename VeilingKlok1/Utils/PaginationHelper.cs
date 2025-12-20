namespace VeilingKlokApp.Utils
{
    /// <summary>
    /// Helper class for pagination calculations
    /// </summary>
    public readonly record struct PaginationInfo(int Page, int Offset, int Limit);

    public static class PaginationHelper
    {
        /// <summary>
        /// Extract the page number from query and calculate offset for pagination.
        /// </summary>
        /// <param name="value">Page number as string from query parameter</param>
        /// <param name="limit">Number of items per page (default: 20)</param>
        /// <returns>PaginationInfo containing page, offset, and limit</returns>
        public static PaginationInfo GetPagination(string? value, int limit = 20)
        {
            int page = 1;

            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out var parsedPage))
            {
                page = Math.Max(parsedPage, 1);
            }

            int offset = (page - 1) * limit;
            return new PaginationInfo(page, offset, limit);
        }

        /// <summary>
        /// Extract the page number from query and calculate offset for pagination.
        /// </summary>
        /// <param name="value">Page number as integer from query parameter</param>
        /// <param name="limit">Number of items per page (default: 20)</param>
        /// <returns>PaginationInfo containing page, offset, and limit</returns>
        public static PaginationInfo GetPagination(int value, int limit = 20)
        {
            int page = Math.Max(value, 1);
            int offset = (page - 1) * limit;
            return new PaginationInfo(page, offset, limit);
        }
    }
}
