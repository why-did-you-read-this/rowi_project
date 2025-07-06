using Microsoft.EntityFrameworkCore;
using rowi_project.Validation;
using System.ComponentModel.DataAnnotations;

namespace rowi_project.Models.Entities
{
    [Index(nameof(Inn), IsUnique = true)]
    [Index(nameof(Ogrn), IsUnique = true)]
    [Index(nameof(ShortName), IsUnique = true)]
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(50, MinimumLength = 6)] // АО "А" - 6 символов
        public string ShortName { get; set; } = null!;

        [Required, StringLength(100, MinimumLength = 6)]
        public string FullName { get; set; } = null!;

        [Required, RegularExpression(@"^\d{10}$")]
        public string Inn { get; set; } = null!;

        [Required, RegularExpression(@"^\d{9}$")]
        public string Kpp { get; set; } = null!;

        [Required, RegularExpression(@"^\d{13}$")]
        public string Ogrn { get; set; } = null!;

        [Required, DateNotInFuture]
        public DateOnly OgrnDateOfAssignment { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string RepName { get; set; } = null!;

        [Required, StringLength(50, MinimumLength = 1)]
        public string RepSurName { get; set; } = null!;

        [StringLength(50)]
        public string? RepPatronymic { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string RepEmail { get; set; } = string.Empty;
        
        [Required, Phone]
        public string RepPhoneNumber { get; set; } = string.Empty;


        public Agent? Agent { get; set; }
        public Bank? Bank { get; set; }
        public Client? Client { get; set; }
    }
}