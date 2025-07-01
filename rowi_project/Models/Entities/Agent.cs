using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    [Table("agents")]
    public class Agent
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        public Company Company { get; set; } = null!;

        [Column("important")]
        public bool Important { get; set; }

        public ICollection<AgentBank> AgentBanks { get; set; } = new List<AgentBank>();
    }
}
