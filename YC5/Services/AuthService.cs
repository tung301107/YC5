using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YC5.Data;
using YC5.DTOs;
using YC5.Interfaces;
using YC5.Models;
using Microsoft.EntityFrameworkCore;

namespace YC5.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> Register(RegisterDto dto)
        {
            // 1. Validation nâng cao
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 5)
                return "Username phải có ít nhất 5 ký tự.";

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return "Mật khẩu phải có ít nhất 6 ký tự.";

            // Kiểm tra username không chứa ký tự đặc biệt (chỉ cho phép chữ và số)
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Username, @"^[a-zA-Z0-9]+$"))
                return "Username không được chứa ký tự đặc biệt.";

            // Kiểm tra trùng lặp Username
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return "Username đã tồn tại";

            // 2. Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tạo User mới
                    var user = new User
                    {
                        Username = dto.Username,
                        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                        FullName = dto.FullName
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync(); // Lưu để lấy được user.Id

                    // 3. Gán Role mặc định (User - Id = 2)
                    var defaultRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = 2 // ID của Role 'User' đã seed trong DBContext
                    };

                    _context.UserRoles.Add(defaultRole);
                    await _context.SaveChangesAsync();

                    // Xác nhận hoàn tất mọi thay đổi
                    await transaction.CommitAsync();
                    return "Đăng ký thành công";
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi nào, hoàn tác (Rollback) toàn bộ dữ liệu đã thêm
                    await transaction.RollbackAsync();
                    return $"Lỗi hệ thống: {ex.Message}";
                }
            }
        }

        public async Task<string?> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return null;

            // Lấy danh sách các Function Code mà User này có quyền
            var permissions = await (from ur in _context.UserRoles
                                     join rf in _context.RoleFunctions on ur.RoleId equals rf.RoleId
                                     join f in _context.Functions on rf.FunctionId equals f.Id
                                     where ur.UserId == user.Id
                                     select f.Code).Distinct().ToListAsync();

            return GenerateJwtToken(user, permissions);
        }

        private string GenerateJwtToken(User user, List<string> permissions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Key"] ?? "DayLaMotChuoiBiMatSieuCapVip123456");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Dán toàn bộ quyền vào Token
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}