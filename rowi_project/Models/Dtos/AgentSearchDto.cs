namespace rowi_project.Models.Dtos
{
    public class AgentSearchDto
    {
        public string? Inn { get; set; }
        public string? RepPhoneNumber { get; set; }
        public string? RepEmail { get; set; }

        public DateOnly? OgrnDateFrom { get; set; }
        public DateOnly? OgrnDateTo { get; set; }

        // Сортировка
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";

        // Пагинация
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
