using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YC5.Filters; // Đảm bảo có namespace này để dùng CheckPermission
using YC5.Interfaces;
using YC5.Models;

namespace YC5.Controllers
{
    [Authorize] // Yêu cầu đăng nhập (JWT Token hợp lệ) cho toàn bộ Controller
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [CheckPermission("STUDENT_VIEW")]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        [CheckPermission("STUDENT_VIEW")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound($"Không tìm thấy sinh viên có ID = {id}");
            return Ok(student);
        }

        [HttpPost]
        [CheckPermission("STUDENT_CREATE")]
        public async Task<IActionResult> Create(Student student)
        {
            var result = await _studentService.AddAsync(student);
            if (result == "Success")
            {
                return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
            }
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [CheckPermission("STUDENT_UPDATE")]
        public async Task<IActionResult> Update(int id, Student student)
        {
            if (id != student.Id) return BadRequest("ID không khớp");

            var updated = await _studentService.UpdateAsync(student);
            if (!updated) return NotFound("Không tìm thấy sinh viên để cập nhật");

            return Ok("Cập nhật thành công");
        }

        [HttpDelete("{id}")]
        [CheckPermission("STUDENT_DELETE")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _studentService.DeleteAsync(id);
            if (!deleted) return NotFound("Không tìm thấy sinh viên để xóa");

            return Ok("Đã xóa sinh viên thành công");
        }



        // 1. Export dữ liệu ra file Excel
        [HttpGet("export")]
        [CheckPermission("STUDENT_EXPORT")]
        public async Task<IActionResult> Export()
        {
            var fileContents = await _studentService.ExportToExcelAsync();
            var fileName = $"Danh_Sach_Sinh_Vien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(
                fileContents,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // 2. Import dữ liệu từ file Excel
        [HttpPost("import")]
        [CheckPermission("STUDENT_IMPORT")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file Excel để upload.");

            var result = await _studentService.ImportFromExcelAsync(file);

            if (result == "Success")
            {
                return Ok(new { message = "Import dữ liệu thành công!" });
            }

            // Nếu có lỗi (validate), result sẽ chứa danh sách các lỗi chi tiết
            return BadRequest(new { error = result });
        }

        [HttpGet("download-template")]
        [CheckPermission("STUDENT_EXPORT")] 
        public async Task<IActionResult> DownloadTemplate()
        {
            var fileContents = await _studentService.GenerateTemplateAsync();
            return File(
                fileContents,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Mau_Nhap_Sinh_Vien.xlsx"
            );
        }

    }
}