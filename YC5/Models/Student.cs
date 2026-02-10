namespace YC5.Models
{
    public class Student
    {
        public int Id { get; set; }

        // MSSV là duy nhất (Unique), chúng ta sẽ cấu hình ở DBContext sau
        public string MSSV { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Major { get; set; } = string.Empty; // Ngành

        public string Class { get; set; } = string.Empty; // Lớp
    }
}
