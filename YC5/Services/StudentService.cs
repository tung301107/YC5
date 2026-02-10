using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // Thư viện EPPlus
using YC5.Data;
using YC5.Interfaces;
using YC5.Models;
using System.Text.RegularExpressions;

namespace YC5.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Các hàm CRUD cơ bản ---
        public async Task<List<Student>> GetAllAsync() => await _context.Students.ToListAsync();

        public async Task<Student?> GetByIdAsync(int id) => await _context.Students.FindAsync(id);

        public async Task<string> AddAsync(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.MSSV == student.MSSV))
                return "Mã số sinh viên đã tồn tại.";

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            var existing = await _context.Students.FindAsync(student.Id);
            if (existing == null) return false;

            existing.FullName = student.FullName;
            existing.DateOfBirth = student.DateOfBirth;
            existing.Email = student.Email;
            existing.Phone = student.Phone;
            existing.Major = student.Major;
            existing.Class = student.Class;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Chức năng EXPORT ---
        public async Task<byte[]> ExportToExcelAsync()
        {
            var students = await _context.Students.ToListAsync();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Students");

            // Header
            string[] headers = { "MSSV", "Họ tên", "Ngày sinh", "Email", "SĐT", "Ngành", "Lớp" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
            }

            // Data
            for (int i = 0; i < students.Count; i++)
            {
                ws.Cells[i + 2, 1].Value = students[i].MSSV;
                ws.Cells[i + 2, 2].Value = students[i].FullName;
                ws.Cells[i + 2, 3].Value = students[i].DateOfBirth.ToString("dd/MM/yyyy");
                ws.Cells[i + 2, 4].Value = students[i].Email;
                ws.Cells[i + 2, 5].Value = students[i].Phone;
                ws.Cells[i + 2, 6].Value = students[i].Major;
                ws.Cells[i + 2, 7].Value = students[i].Class;
            }
            ws.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }

        // --- Chức năng IMPORT + VALIDATE ---
        public async Task<string> ImportFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0) return "File trống!";
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return "Định dạng file không hỗ trợ! Vui lòng dùng .xlsx";

            var listImport = new List<Student>();
            var errors = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                int rowCount = ws.Dimension?.Rows ?? 0;

                if (rowCount < 2) return "File không có dữ liệu!";

                for (int row = 2; row <= rowCount; row++)
                {
                    var mssv = ws.Cells[row, 1].Value?.ToString()?.Trim();
                    var name = ws.Cells[row, 2].Value?.ToString()?.Trim();
                    var dobStr = ws.Cells[row, 3].Value?.ToString()?.Trim();
                    var email = ws.Cells[row, 4].Value?.ToString()?.Trim();

                    // --- VALIDATION LOGIC ---
                    if (string.IsNullOrEmpty(mssv)) errors.Add($"Dòng {row}: MSSV trống");
                    if (string.IsNullOrEmpty(name)) errors.Add($"Dòng {row}: Họ tên trống");

                    // Check định dạng Email sơ bộ
                    if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        errors.Add($"Dòng {row}: Email sai định dạng");

                    // Check định dạng ngày sinh
                    if (!DateTime.TryParse(dobStr, out DateTime dob))
                        errors.Add($"Dòng {row}: Ngày sinh sai định dạng (Cần dd/MM/yyyy hoặc MM/dd/yyyy)");

                    // Check trùng MSSV trong DB
                    if (await _context.Students.AnyAsync(s => s.MSSV == mssv))
                        errors.Add($"Dòng {row}: MSSV {mssv} đã tồn tại trong hệ thống");

                    if (errors.Count == 0)
                    {
                        listImport.Add(new Student
                        {
                            MSSV = mssv!,
                            FullName = name!,
                            DateOfBirth = dob,
                            Email = email ?? "",
                            Phone = ws.Cells[row, 5].Value?.ToString() ?? "",
                            Major = ws.Cells[row, 6].Value?.ToString() ?? "",
                            Class = ws.Cells[row, 7].Value?.ToString() ?? ""
                        });
                    }
                }
            }

            if (errors.Any()) return string.Join(" | ", errors);

            _context.Students.AddRange(listImport);
            await _context.SaveChangesAsync();
            return "Success";
        }
        public async Task<byte[]> GenerateTemplateAsync()
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Template");

            // 1. Định nghĩa Headers
            string[] headers = { "MSSV", "Họ tên", "Ngày sinh", "Email", "SĐT", "Ngành", "Lớp" };

            // 2. Định dạng Header (Màu sắc, chữ đậm)
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                ws.Cells[1, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            }

            // 3. Thêm một dòng ví dụ (Dòng này sẽ không được import vào DB hoặc nhắc người dùng xóa đi)
            ws.Cells[2, 1].Value = "SV001";
            ws.Cells[2, 2].Value = "Nguyễn Văn A";
            ws.Cells[2, 3].Value = "01/01/2000";
            ws.Cells[2, 4].Value = "vana@gmail.com";
            ws.Cells[2, 5].Value = "0987654321";
            ws.Cells[2, 6].Value = "CNTT";
            ws.Cells[2, 7].Value = "D15-01";

            // Thêm ghi chú (Comment) cho cột Ngày sinh để người dùng nhập đúng format
            var comment = ws.Cells[2, 3].AddComment("Định dạng: dd/MM/yyyy", "System");

            ws.Cells.AutoFitColumns();
            return await package.GetAsByteArrayAsync();
        }
    }
}