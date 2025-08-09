using AutoMapper;
using Learning_Management_System.DTOs.Course;
using Learning_Management_System.DTOs.Enrollment;
using Learning_Management_System.DTOs.Lesson;
using Learning_Management_System.DTOs.Quiz;
using Learning_Management_System.Models;

namespace Learning_Management_System.Mapping
{
    public class LmsMappingProfile : Profile
    {
        public LmsMappingProfile()
        {
            // DTO -> Entity
            CreateMap<CourseCreateDto, Course>();
            CreateMap<CourseUpdateDto, Course>();

            // Entity -> DTO (LIST ITEM)
            CreateMap<Course, CourseListItemDto>()
                .ForMember(d => d.InstructorName, o => o.MapFrom(s => s.Instructor.FullName));

            // Nested maps (თუ დეტალებში გჭირდება)
            CreateMap<Lesson, LessonDto>();
            CreateMap<Quiz, QuizDto>();

            // Entity -> DTO (DETAIL)
            CreateMap<Course, CourseDetailDto>()
                .IncludeBase<Course, CourseListItemDto>()  // იღებს ზემოს წესებსაც
                .ForMember(d => d.Lessons, o => o.MapFrom(s => s.Lessons))
                .ForMember(d => d.Quizzes, o => o.MapFrom(s => s.Quizzes))
                .ForMember(d => d.IsEnrolled, o => o.Ignore()); // ამას სერვისში შეავსებ



            // Enrollment -> EnrollmentDto
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(d => d.CourseId, o => o.MapFrom(s => s.CourseId))
                .ForMember(d => d.CourseTitle, o => o.MapFrom(s => s.Course.Title))
                .ForMember(d => d.Progress, o => o.MapFrom(s => s.Progress));

            CreateMap<LessonCreateDto, Lesson>();

            // Entity -> DTO
            CreateMap<Lesson, LessonDto>();

            CreateMap<QuizCreateDto, Quiz>();     // <- ეს გაკლდა

            // Entity -> DTO (თუ იყენებ სადმე)
            CreateMap<Quiz, QuizDto>();
        }


    }
}
