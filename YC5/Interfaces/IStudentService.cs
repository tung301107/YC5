using YC5.Models;

namespace YC5.Interfaces
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<string> AddAsync(Student student); // Trả về thông báo lỗi hoặc "Success"
        Task<bool> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<byte[]> ExportToExcelAsync(); // Mới
        Task<string> ImportFromExcelAsync(IFormFile file); // Mới
        Task<byte[]> GenerateTemplateAsync();
    }
}