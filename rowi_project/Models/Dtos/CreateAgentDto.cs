using rowi_project.Validation;
using System.ComponentModel.DataAnnotations;

namespace rowi_project.Models.Dtos
{
    public class CreateAgentDto
    {
        // Параметры компании
        [Required]
        public string ShortName { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{10}$")]
        public string Inn { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{9}$")]
        public string Kpp { get; set; } = null!;

        [Required]
        [RegularExpression(@"^\d{13}$")]
        public string Ogrn { get; set; } = null!;

        [Required]
        [DateNotInFuture]
        public DateOnly OgrnDateOfAssignment { get; set; }

        // Представитель
        [Required]
        public string RepSurname { get; set; } = null!;

        [Required]
        public string RepName { get; set; } = null!;

        public string RepPatronymic { get; set; } = string.Empty;

        [EmailAddress]
        public string RepEmail { get; set; } = string.Empty;

        [Phone]
        public string RepPhoneNumber { get; set; } = string.Empty;

        // Доп. параметры
        public List<int> BankIds { get; set; } = [];
        public bool Important { get; set; }
    }
}
