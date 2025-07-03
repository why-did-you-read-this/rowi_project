using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public Company Company { get; set; } = null!;
    }
}
