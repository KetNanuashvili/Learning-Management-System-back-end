namespace Learning_Management_System.DTOs.Statistics
{
    public class ExportRequestDto
    {
        public string Format { get; set; } = "pdf"; // or csv, excel
        public string ReportType { get; set; } = "summary"; // summary / detailed
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
