using AutoMapper;
using Learning_Management_System.Data;
using Learning_Management_System.DTOs.Course;
using Learning_Management_System.DTOs.Enrollment;
using Learning_Management_System.DTOs.Lesson;
using Learning_Management_System.DTOs.Message;
using Learning_Management_System.DTOs.Quiz;
using Learning_Management_System.DTOs.Statistics;
using Learning_Management_System.Models;
using Learning_Management_System.Repositories.LMS;
using Microsoft.EntityFrameworkCore;

namespace Learning_Management_System.Services.LMSServiceFile
{
    public class LMSService : ILMSService
    {
        private readonly ILMSRepository _repository;
        private readonly IMapper _mapper;

        public LMSService(ILMSRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ----- Courses -----
        public async Task<IEnumerable<CourseListItemDto>> GetAllCoursesAsync()
        {
            var courses = await _repository.GetAllCoursesAsync();
            return _mapper.Map<IEnumerable<CourseListItemDto>>(courses);
        }

        public async Task<CourseDetailDto?> GetCourseByIdAsync(int id)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            return course == null ? null : _mapper.Map<CourseDetailDto>(course);
        }

        public async Task<CourseDetailDto> CreateCourseAsync(CourseCreateDto dto)
        {
            var course = _mapper.Map<Course>(dto);
            var created = await _repository.AddCourseAsync(course);
            var createdFull = await _repository.GetCourseByIdAsync(created.Id);
            return _mapper.Map<CourseDetailDto>(createdFull);
        }

        public async Task<bool> UpdateCourseAsync(int id, CourseUpdateDto dto)
        {
            var course = await _repository.GetCourseByIdAsync(id);
            if (course == null) return false;

            _mapper.Map(dto, course);
            return await _repository.UpdateCourseAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            return await _repository.DeleteCourseAsync(id);
        }

        // ----- Enrollments -----
        public async Task<bool> EnrollStudentAsync(EnrollRequestDto dto, int studentId)
        {
            if (await _repository.IsAlreadyEnrolledAsync(dto.CourseId, studentId))
                return false;

            var enrollment = new Enrollment
            {
                CourseId = dto.CourseId,
                StudentId = studentId,
                Progress = 0
            };

            return await _repository.EnrollStudentAsync(enrollment);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetUserEnrollmentsAsync(int studentId)
        {
            var enrollments = await _repository.GetUserEnrollmentsAsync(studentId);
            return _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
        }

        // ----- Lessons -----
        public async Task<LessonDto> CreateLessonAsync(LessonCreateDto dto)
        {
            var lesson = _mapper.Map<Lesson>(dto);
            var saved = await _repository.AddLessonAsync(lesson);
            return _mapper.Map<LessonDto>(saved);
        }

        public async Task<LessonDto?> GetLessonByIdAsync(int id)
        {
            var lesson = await _repository.GetLessonByIdAsync(id);
            return lesson == null ? null : _mapper.Map<LessonDto>(lesson);
        }

        // ----- Quizzes -----
        public async Task<QuizDto> CreateQuizAsync(QuizCreateDto dto)
        {
            var quiz = _mapper.Map<Quiz>(dto);
            var saved = await _repository.AddQuizAsync(quiz);
            return _mapper.Map<QuizDto>(saved);
        }

        public async Task<bool> AddQuestionToQuizAsync(int quizId, QuizQuestionCreateDto dto)
        {
            var question = _mapper.Map<QuizQuestion>(dto);
            question.QuizId = quizId;
            return await _repository.AddQuestionToQuizAsync(question);
        }

        public async Task<bool> SubmitQuizAsync(int quizId, QuizSubmissionDto dto, int studentId)
        {
            var result = new QuizResult
            {
                QuizId = quizId,
                StudentId = studentId,
                Score = 100 // Placeholder
            };
            return await _repository.SubmitQuizAsync(result);
        }

        public async Task<QuizResultDto?> GetQuizResultAsync(int quizId, int studentId)
        {
            var result = await _repository.GetQuizResultAsync(quizId, studentId);
            return result == null ? null : _mapper.Map<QuizResultDto>(result);
        }

        // ----- Messages -----
        public async Task<IEnumerable<MessageDto>> GetMessagesAsync(int courseId)
        {
            var messages = await _repository.GetMessagesAsync(courseId);
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageDto dto, int senderId)
        {
            var message = new Message
            {
                CourseId = dto.CourseId,
                Content = dto.Content,
                SenderId = senderId,
                SentAt = DateTime.UtcNow
            };

            var saved = await _repository.AddMessageAsync(message);
            return _mapper.Map<MessageDto>(saved);
        }

        // ----- Reports -----
        public async Task<ProgressChartDto> GetStatisticsAsync()
        {
            var enrolled = await _repository.GetTotalEnrollmentsAsync();
            var quizzes = await _repository.GetTotalQuizzesAsync();

            return new ProgressChartDto
            {
                TotalEnrollments = enrolled,
                TotalQuizzes = quizzes
            };
        }

        public async Task<byte[]> ExportToPdfAsync(ExportRequestDto dto) => new byte[0];
        public async Task<byte[]> ExportToExcelAsync(ExportRequestDto dto) => new byte[0];
    }

}
