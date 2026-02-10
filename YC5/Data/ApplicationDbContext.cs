using Microsoft.EntityFrameworkCore;
using YC5.Models;

namespace YC5.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<UserFunction> UserFunctions { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // Đã sửa từ ModelCreatingBuilder
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Khóa chính kết hợp cho các bảng trung gian
            modelBuilder.Entity<RoleFunction>().HasKey(rf => new { rf.RoleId, rf.FunctionId });
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserFunction>().HasKey(uf => new { uf.UserId, uf.FunctionId });

            // Cấu hình MSSV là duy nhất
            modelBuilder.Entity<Student>().HasIndex(s => s.MSSV).IsUnique();

            // --- SEED DATA ---
            // 1. Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrator" },
                new Role { Id = 2, Name = "User" }
            );

            // 2. Seed Functions
            modelBuilder.Entity<Function>().HasData(
                new Function { Id = 1, Code = "STUDENT_VIEW", Name = "Xem danh sách sinh viên" },
                new Function { Id = 2, Code = "STUDENT_CREATE", Name = "Thêm mới sinh viên" },
                new Function { Id = 3, Code = "STUDENT_UPDATE", Name = "Cập nhật sinh viên" },
                new Function { Id = 4, Code = "STUDENT_DELETE", Name = "Xóa sinh viên" },
                new Function { Id = 5, Code = "STUDENT_IMPORT", Name = "Import sinh viên" },
                new Function { Id = 6, Code = "STUDENT_EXPORT", Name = "Export sinh viên" }
            );

            // 3. Seed RoleFunctions (Gán tất cả quyền cho Admin)
            for (int i = 1; i <= 6; i++)
            {
                modelBuilder.Entity<RoleFunction>().HasData(
                    new RoleFunction { RoleId = 1, FunctionId = i }
                );
            }

            // 4. Seed Sinh viên mẫu
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, MSSV = "SV001", FullName = "Nguyễn Văn An", DateOfBirth = new DateTime(2002, 5, 15), Email = "an.nv@gmail.com", Phone = "0912345678", Major = "CNTT", Class = "D15-01" },
                new Student { Id = 2, MSSV = "SV002", FullName = "Trần Thị Bình", DateOfBirth = new DateTime(2002, 10, 20), Email = "binh.tt@gmail.com", Phone = "0988776655", Major = "Kế toán", Class = "K15-02" }
            );
        }
    }
}