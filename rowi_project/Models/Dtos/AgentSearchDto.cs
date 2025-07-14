namespace rowi_project.Models.Dtos;

public record AgentSearchDto
{
    public string? Inn { get; set; }
    public string? Ogrn { get; set; }
    public string? RepPhoneNumber { get; set; }
    public string? RepEmail { get; set; }
    public string? ShortName { get; set; }
    public string? RepName { get; set; }
    public string? RepSurName { get; set; }
    public bool? Important { get; set; }

    public DateOnly? OgrnDateFrom { get; set; }
    public DateOnly? OgrnDateTo { get; set; }

    // Сортировка
    public string? SortBy { get; set; } = "id";
    public string? SortDirection { get; set; } = "asc";

    // Пагинация
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}
