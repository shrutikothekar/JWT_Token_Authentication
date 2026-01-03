namespace doctor_app_api.Models
{
    public enum AppointmentStatus { Pending, Confirmed, Cancelled }

    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;

        // Prevent double booking: unique (DoctorId, StartUtc)
    }
}
