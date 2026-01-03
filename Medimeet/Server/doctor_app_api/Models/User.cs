using System.ComponentModel.DataAnnotations;

namespace doctor_app_api.Models
{
    public enum UserRole { Patient, Doctor, Admin }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(120)]
        public string FullName { get; set; } = default!;
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string PasswordHash { get; set; } = default!;
        public UserRole Role { get; set; } = UserRole.Patient;

        // Doctor-only fields (nullable for Patients)
        public string? Specialization { get; set; }
        public string? Bio { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}
