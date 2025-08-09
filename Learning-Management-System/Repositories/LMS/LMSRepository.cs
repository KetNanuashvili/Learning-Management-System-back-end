using Learning_Management_System.Data;
using Learning_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Learning_Management_System.Repositories.LMS
{
    public class LMSRepository : ILMSRepository
    {
        private readonly AppDbContext _context;

        public LMSRepository(AppDbContext context)
        {
            _context = context;
        }

        // ----- Courses -----
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Instructor) 
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
            => await _context.Courses.Include(c => c.Lessons).Include(c => c.Quizzes).Include(c => c.Instructor).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Course> AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        // ----- Enrollments -----
        public async Task<bool> IsAlreadyEnrolledAsync(int courseId, int studentId)
            => await _context.Enrollments.AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);

        public async Task<bool> EnrollStudentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(int studentId)
            => await _context.Enrollments.Include(e => e.Course).Where(e => e.StudentId == studentId).ToListAsync();

        // ----- Lessons -----
        public async Task<Lesson> AddLessonAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson?> GetLessonByIdAsync(int id)
            => await _context.Lessons.FindAsync(id);

        // ----- Quizzes -----
        public async Task<Quiz> AddQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> AddQuestionToQuizAsync(QuizQuestion question)
        {
            _context.QuizQuestions.Add(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitQuizAsync(QuizResult result)
        {
            _context.QuizResults.Add(result);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<QuizResult?> GetQuizResultAsync(int quizId, int studentId)
            => await _context.QuizResults.FirstOrDefaultAsync(r => r.QuizId == quizId && r.StudentId == studentId);

        // ----- Messages -----
        public async Task<IEnumerable<Message>> GetMessagesAsync(int courseId)
            => await _context.Messages.Where(m => m.CourseId == courseId).OrderBy(m => m.SentAt).ToListAsync();

        public async Task<Message> AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        // ----- Statistics -----
        public async Task<int> GetTotalEnrollmentsAsync() => await _context.Enrollments.CountAsync();
        public async Task<int> GetTotalQuizzesAsync() => await _context.Quizzes.CountAsync();
    }
}
