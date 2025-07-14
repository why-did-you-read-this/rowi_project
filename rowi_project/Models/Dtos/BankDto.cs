namespace rowi_project.Models.Dtos;

public record BankDto
{
    public int Id { get; set; }
    public string ShortName { get; set; } = null!;
}
