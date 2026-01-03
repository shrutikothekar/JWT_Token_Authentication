namespace doctor_app_api.Models
{
    public class Clinic
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
    }
}
