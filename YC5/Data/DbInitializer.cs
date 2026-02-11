using Microsoft.EntityFrameworkCore;
using YC5.Models;

namespace YC5.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 1. Đảm bảo các Migration mới nhất (bao gồm Composite Key) đã được áp dụng
            context.Database.Migrate();

            // 2. Kiểm tra nếu đã có dữ liệu User thì dừng lại để tránh trùng lặp
            if (context.Users.Any())
            {
                return;
            }

            // 3. Seed Roles
            var roles = new Role[]
            {
                new Role { Name = "Administrator" },
                new Role { Name = "User" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // 4. Seed Admin User (Password: Admin123!)
            var adminUser = new User
            {
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "Hệ Thống Admin",
                IsActive = true
            };
            context.Users.Add(adminUser);
            context.SaveChanges();

            // 5. Gán Role Administrator cho Admin (Bảng UserRole)
            var adminRole = context.Roles.First(r => r.Name == "Administrator");
            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            context.SaveChanges();

            // 6. Seed Functions
            var functions = new Function[]
            {
                new Function { Code = "STUDENT_VIEW", Name = "Xem danh sách sinh viên" },
                new Function { Code = "STUDENT_CREATE", Name = "Thêm mới sinh viên" },
                new Function { Code = "STUDENT_UPDATE", Name = "Cập nhật sinh viên" },
                new Function { Code = "STUDENT_DELETE", Name = "Xóa sinh viên" },
                new Function { Code = "STUDENT_IMPORT", Name = "Import sinh viên" },
                new Function { Code = "STUDENT_EXPORT", Name = "Export sinh viên" }
            };
            context.Functions.AddRange(functions);
            context.SaveChanges();

            // 7. Gán tất cả Functions cho Role Administrator (Bảng RoleFunction)
            var allFunctions = context.Functions.ToList();
            foreach (var function in allFunctions)
            {
                context.RoleFunctions.Add(new RoleFunction
                {
                    RoleId = adminRole.Id,
                    FunctionId = function.Id
                });
            }
            context.SaveChanges();

            // 8. Seed Sinh viên mẫu
            var students = new Student[]
            {
                new Student
                {
                    MSSV = "SV001",
                    FullName = "Nguyễn Văn An",
                    DateOfBirth = new DateTime(2002, 5, 15),
                    Email = "an.nv@gmail.com",
                    Phone = "0912345678",
                    Major = "CNTT",
                    Class = "D15-01"
                },
                new Student
                {
                    MSSV = "SV002",
                    FullName = "Trần Thị Bình",
                    DateOfBirth = new DateTime(2002, 10, 20),
                    Email = "binh.tt@gmail.com",
                    Phone = "0988776655",
                    Major = "Kế toán",
                    Class = "K15-02"
                }
            };
            context.Students.AddRange(students);
            context.SaveChanges();
        }
    }
}