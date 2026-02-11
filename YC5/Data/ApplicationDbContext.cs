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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính phức hợp cho các bảng trung gian
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<RoleFunction>()
                .HasKey(rf => new { rf.RoleId, rf.FunctionId });

            modelBuilder.Entity<UserFunction>()
                .HasKey(uf => new { uf.UserId, uf.FunctionId });

            // Cấu hình MSSV của Student là Unique (như bạn đã note trong Student.cs)
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.MSSV)
                .IsUnique();
        }
    }
}