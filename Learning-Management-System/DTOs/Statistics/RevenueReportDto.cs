namespace Learning_Management_System.DTOs.Statistics
{
    public class RevenueReportDto
    {

        public string CourseTitle { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public DateTime Month { get; set; }
    }
}
