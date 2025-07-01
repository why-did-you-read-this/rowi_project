using rowi_project.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rowi_project.Models.Entities
{
    [Table("companies")]
    public class Company
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("short_name")]
        [StringLength(50, MinimumLength = 6)] // АО "А" - 6 символов
        public string ShortName { get; set; } = null!;

        [Column("full_name")]
        [StringLength(100, MinimumLength = 6)]
        public string FullName { get; set; } = null!;

        [Column("inn")]
        [RegularExpression(@"^\d{10}$")]
        public string Inn { get; set; } = null!;

        [Column("kpp")]
        [RegularExpression(@"^\d{9}$")]
        public string Kpp { get; set; } = null!;

        [Column("ogrn")]
        [RegularExpression(@"^\d{13}$")]
        public string Ogrn { get; set; } = null!;

        [Column("ogrn_date_of_assignment")]
        [DateNotInFuture]
        public DateOnly OgrnDateOfAssignment { get; set; }

        [Column("rep_name")]
        [StringLength(50, MinimumLength = 1)]
        public string RepName { get; set; } = null!;

        [Column("rep_surname")]
        [StringLength(50, MinimumLength = 1)]
        public string RepSurName { get; set; } = null!;

        [Column("rep_patronymic")]
        [StringLength(50)]
        public string RepPatronymic { get; set; } = string.Empty;

        [Column("rep_email")]
        [EmailAddress]
        public string RepEmail { get; set; } = string.Empty;
        
        [Column("rep_phone_number")]
        [Phone]
        public string RepPhoneNumber { get; set; } = string.Empty;


        public Agent? Agent { get; set; }
        public Bank? Bank { get; set; }
        public Client? Client { get; set; }
    }
}