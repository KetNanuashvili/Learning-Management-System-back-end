using Learning_Management_System.DTOs.Course;
using Learning_Management_System.DTOs.Enrollment;
using Learning_Management_System.DTOs.Lesson;
using Learning_Management_System.DTOs.Message;
using Learning_Management_System.DTOs.Quiz;
using Learning_Management_System.DTOs.Statistics;
using Learning_Management_System.Services.Interfaces;
using Learning_Management_System.Services.LMSServiceFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learning_Management_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LMSController : ControllerBase
    {
        private readonly ILMSService _lmsService;

        public LMSController(ILMSService lmsService)
        {
            _lmsService = lmsService;
        }

        // ----- Courses -----

        [HttpGet("courses")]
        [AllowAnonymous] // ყველა მომხმარებლისთვის
        public async Task<ActionResult<IEnumerable<CourseListItemDto>>> GetAllCourses()
        {
            return Ok(await _lmsService.GetAllCoursesAsync());
        }

        [HttpGet("courses/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDetailDto>> GetCourseById(int id)
        {
            var result = await _lmsService.GetCourseByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("courses")]
        [Authorize(Roles = "Admin")] // მხოლოდ ადმინს შეუძლია შექმნა
        public async Task<ActionResult<CourseDetailDto>> CreateCourse([FromBody] CourseCreateDto dto)
        {
            var result = await _lmsService.CreateCourseAsync(dto);
            return CreatedAtAction(nameof(GetCourseById), new { id = result.Id }, result);
        }

        [HttpPut("courses/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseUpdateDto dto)
        {
            var success = await _lmsService.UpdateCourseAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("courses/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await _lmsService.DeleteCourseAsync(id);
            return success ? NoContent() : NotFound();
        }

        // ----- Enrollment -----

        [HttpPost("enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EnrollStudent([FromBody] EnrollRequestDto dto, [FromQuery] int studentId)
        {
            var success = await _lmsService.EnrollStudentAsync(dto, studentId);
            return success ? Ok() : Conflict("Student is already enrolled.");
        }

        [HttpGet("enrollments/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetUserEnrollments(int studentId)
        {
            return Ok(await _lmsService.GetUserEnrollmentsAsync(studentId));
        }

        // ----- Lessons -----

        [HttpPost("lessons")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<LessonDto>> CreateLesson([FromBody] LessonCreateDto dto)
        {
            var result = await _lmsService.CreateLessonAsync(dto);
            return Ok(result);
        }

        [HttpGet("lessons/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<LessonDto>> GetLessonById(int id)
        {
            var lesson = await _lmsService.GetLessonByIdAsync(id);
            return lesson == null ? NotFound() : Ok(lesson);
        }

        // ----- Quizzes -----

        [HttpPost("quizzes")]
        [Authorize(Roles = "Instructor")]
        public async Task<ActionResult<QuizDto>> CreateQuiz([FromBody] QuizCreateDto dto)
        {
            return Ok(await _lmsService.CreateQuizAsync(dto));
        }

        [HttpPost("quizzes/{quizId}/questions")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> AddQuestionToQuiz(int quizId, [FromBody] QuizQuestionCreateDto dto)
        {
            var success = await _lmsService.AddQuestionToQuizAsync(quizId, dto);
            return success ? Ok() : BadRequest();
        }

        [HttpPost("quizzes/{quizId}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitQuiz(int quizId, [FromBody] QuizSubmissionDto dto, [FromQuery] int studentId)
        {
            var success = await _lmsService.SubmitQuizAsync(quizId, dto, studentId);
            return success ? Ok() : BadRequest();
        }

        [HttpGet("quizzes/{quizId}/results/{studentId}")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<QuizResultDto>> GetQuizResult(int quizId, int studentId)
        {
            var result = await _lmsService.GetQuizResultAsync(quizId, studentId);
            return result == null ? NotFound() : Ok(result);
        }

        // ----- Messages -----

        [HttpGet("courses/{courseId}/messages")]
        [Authorize(Roles = "Student,Instructor,Admin")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int courseId)
        {
            return Ok(await _lmsService.GetMessagesAsync(courseId));
        }

        [HttpPost("messages")]
        [Authorize(Roles = "Student,Instructor")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto, [FromQuery] int senderId)
        {
            return Ok(await _lmsService.SendMessageAsync(dto, senderId));
        }

        // ----- Statistics & Reports -----

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProgressChartDto>> GetStatistics()
        {
            return Ok(await _lmsService.GetStatisticsAsync());
        }

        [HttpPost("export/pdf")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportToPdf([FromBody] ExportRequestDto dto)
        {
            var data = await _lmsService.ExportToPdfAsync(dto);
            return File(data, "application/pdf", "report.pdf");
        }

        [HttpPost("export/excel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportToExcel([FromBody] ExportRequestDto dto)
        {
            var data = await _lmsService.ExportToExcelAsync(dto);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "report.xlsx");
        }
    }
}
