using Learning_Management_System.DTOs.Course;
using Learning_Management_System.DTOs.Enrollment;
using Learning_Management_System.DTOs.Lesson;
using Learning_Management_System.DTOs.Message;
using Learning_Management_System.DTOs.Quiz;
using Learning_Management_System.DTOs.Statistics;

namespace Learning_Management_System.Services.LMSServiceFile
{
    public interface ILMSService
    {
        // Courses
        Task<IEnumerable<CourseListItemDto>> GetAllCoursesAsync();
        Task<CourseDetailDto?> GetCourseByIdAsync(int id);
        Task<CourseDetailDto> CreateCourseAsync(CourseCreateDto dto);
        Task<bool> UpdateCourseAsync(int id, CourseUpdateDto dto);
        Task<bool> DeleteCourseAsync(int id);

        // Enrollments
        Task<bool> EnrollStudentAsync(EnrollRequestDto dto, int studentId);
        Task<IEnumerable<EnrollmentDto>> GetUserEnrollmentsAsync(int studentId);

        // Lessons
        Task<LessonDto> CreateLessonAsync(LessonCreateDto dto);
        Task<LessonDto?> GetLessonByIdAsync(int id);

        // Quizzes
        Task<QuizDto> CreateQuizAsync(QuizCreateDto dto);
        Task<bool> AddQuestionToQuizAsync(int quizId, QuizQuestionCreateDto dto);
        Task<bool> SubmitQuizAsync(int quizId, QuizSubmissionDto dto, int studentId);
        Task<QuizResultDto?> GetQuizResultAsync(int quizId, int studentId);

        // Messages
        Task<IEnumerable<MessageDto>> GetMessagesAsync(int courseId);
        Task<MessageDto> SendMessageAsync(SendMessageDto dto, int senderId);

        // Reports
        Task<ProgressChartDto> GetStatisticsAsync();
        Task<byte[]> ExportToPdfAsync(ExportRequestDto dto);
        Task<byte[]> ExportToExcelAsync(ExportRequestDto dto);
    }
}
