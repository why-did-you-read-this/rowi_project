namespace rowi_project.Models.Dtos;

public record AgentDto
{
    public int Id { get; set; }

    // Представитель
    public string RepFullName { get; set; } = null!;
    public string RepEmail { get; set; } = null!;
    public string RepPhoneNumber { get; set; } = null!;

    // Компания
    public string ShortName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Inn { get; set; } = null!;
    public string Kpp { get; set; } = null!;
    public string Ogrn { get; set; } = null!;
    public DateOnly OgrnDateOfAssignment { get; set; }

    // Банки
    public List<BankDto> Banks { get; set; } = [];

    public bool Important { get; set; }
}
