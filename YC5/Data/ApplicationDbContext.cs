using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using YC5.Models;

namespace YC5.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Đăng ký các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserFunction> UserFunctions { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình Khóa chính cho các bảng trung gian (Composite Key)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserFunction>()
                .HasKey(uf => new { uf.UserId, uf.FunctionId });

            modelBuilder.Entity<RoleFunction>()
                .HasKey(rf => new { rf.RoleId, rf.FunctionId });

            // 2. Cấu hình Student: MSSV phải là duy nhất (Unique)
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.MSSV)
                .IsUnique();

            // 3. (Tùy chọn) Seed dữ liệu mẫu cho Function để test quyền
            modelBuilder.Entity<Function>().HasData(
                new Function { Id = 1, Name = "Import Sinh Viên", Code = "STUDENT_IMPORT" },
                new Function { Id = 2, Name = "Export Sinh Viên", Code = "STUDENT_EXPORT" },
                new Function { Id = 3, Name = "Xem Sinh Viên", Code = "STUDENT_VIEW" }
            );
        }
    }
}