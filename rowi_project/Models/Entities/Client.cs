using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    [Table("clients")]
    public class Client
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        public Company Company { get; set; } = null!;
    }

}
