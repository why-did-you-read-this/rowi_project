using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public bool Important { get; set; }

        public Company Company { get; set; } = null!;

        public ICollection<Bank> Banks { get; set; } = [];
    }
}
