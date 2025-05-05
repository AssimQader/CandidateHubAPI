using CandidateHub.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandidateHub.Domain.Entities
{
    [Table("Candidate")]
    public class Candidate
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format!")]
        public required string PhoneNumber { get; set; }

        public TimeIntervalPreference? CallTimePreference { get; set; }

        [MaxLength(255)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(255)]
        public string? GitHubUrl { get; set; }

        [MaxLength(2000)]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
