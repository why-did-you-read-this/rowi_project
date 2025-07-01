using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    [Table("agents_banks")]
    public class AgentBank
    {
        [Column("agent_id")]
        public int AgentId { get; set; }
        public Agent Agent { get; set; } = null!;

        [Column("bank_id")]
        public int BankId { get; set; }
        public Bank Bank { get; set; } = null!;
    }
}
