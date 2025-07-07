using System.ComponentModel.DataAnnotations;

namespace rowi_project.Models.Entities;
public class Agent
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool Important { get; set; }

    public Company Company { get; set; } = null!;

    public ICollection<Bank> Banks { get; set; } = [];
}
