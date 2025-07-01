using System.ComponentModel.DataAnnotations;

namespace rowi_project.Models.Dtos
{
    public class UpdateAgentDto : CreateAgentDto
    {
        [Required]
        public int Id { get; set; }
    }

}
