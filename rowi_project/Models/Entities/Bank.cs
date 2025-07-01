using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    [Table("banks")]
    public class Bank
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        public Company Company { get; set; } = null!;

        public ICollection<AgentBank> AgentBanks { get; set; } = new List<AgentBank>();
    }


}
