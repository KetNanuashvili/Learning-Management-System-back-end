using Learning_Management_System.Models;

namespace Learning_Management_System.Repositories.LMS
{
    public interface ILMSRepository
    {
        // Courses
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> AddCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);

        // Enrollments
        Task<bool> IsAlreadyEnrolledAsync(int courseId, int studentId);
        Task<bool> EnrollStudentAsync(Enrollment enrollment);
        Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(int studentId);

        // Lessons
        Task<Lesson> AddLessonAsync(Lesson lesson);
        Task<Lesson?> GetLessonByIdAsync(int id);

        // Quizzes
        Task<Quiz> AddQuizAsync(Quiz quiz);
        Task<bool> AddQuestionToQuizAsync(QuizQuestion question);
        Task<bool> SubmitQuizAsync(QuizResult result);
        Task<QuizResult?> GetQuizResultAsync(int quizId, int studentId);

        // Messages
        Task<IEnumerable<Message>> GetMessagesAsync(int courseId);
        Task<Message> AddMessageAsync(Message message);

        // Statistics
        Task<int> GetTotalEnrollmentsAsync();
        Task<int> GetTotalQuizzesAsync();
    }
}
