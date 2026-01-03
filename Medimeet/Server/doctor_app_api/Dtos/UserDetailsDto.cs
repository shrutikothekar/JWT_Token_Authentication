namespace doctor_app_api.Dtos
{
    public class UserDetailsDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // Doctor Specific (NA if not Doctor)
        public string Specialization { get; set; }
        public string ClinicName { get; set; }
        public string ClinicAddress { get; set; }
    }

}
