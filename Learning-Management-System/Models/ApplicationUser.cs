using Microsoft.AspNetCore.Identity;

namespace Learning_Management_System.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = "Student";

        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>(); // 👈 ასწავლის ამ კურსებს
        public ICollection<Message>? Messages { get; set; }
        public ICollection<QuizResult>? QuizResults { get; set; }

        // Seed roles method
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "Admin", "Instructor", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }
    }
}
