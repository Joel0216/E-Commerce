namespace Application.DTOs
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public PaginationDto Pagination { get; set; } = new PaginationDto();
    }
} 