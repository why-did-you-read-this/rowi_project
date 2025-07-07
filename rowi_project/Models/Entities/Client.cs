using System.ComponentModel.DataAnnotations;

namespace rowi_project.Models.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public Company Company { get; set; } = null!;
    }
}
