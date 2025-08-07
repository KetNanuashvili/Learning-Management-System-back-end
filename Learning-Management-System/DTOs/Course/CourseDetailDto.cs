using Learning_Management_System.DTOs.Lesson;
using Learning_Management_System.DTOs.Quiz;

namespace Learning_Management_System.DTOs.Course
{
    public class CourseDetailDto : CourseListItemDto
    {
        public string Description { get; set; } = string.Empty;
        public List<LessonDto> Lessons { get; set; } = new();
        public List<QuizDto> Quizzes { get; set; } = new();
        public bool IsEnrolled { get; set; }
    }
}
