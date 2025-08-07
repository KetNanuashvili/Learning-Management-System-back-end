namespace Learning_Management_System.DTOs.User
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
    }
}
