using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YC5.Data;

namespace YC5.Filters
{
    public class PermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly string _functionCode;
        private readonly ApplicationDbContext _context;

        public PermissionFilter(string functionCode, ApplicationDbContext context)
        {
            _functionCode = functionCode;
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 1. Lấy UserId từ Claims trong Token
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userId = int.Parse(userIdClaim.Value);

            // 2. Kiểm tra quyền trong DB
            // Check quyền trực tiếp của User OR Quyền thông qua Role của User đó
            var hasPermission = await (from uf in _context.UserFunctions
                                       join f in _context.Functions on uf.FunctionId equals f.Id
                                       where uf.UserId == userId && f.Code == _functionCode
                                       select f).AnyAsync()
                                || await (from ur in _context.UserRoles
                                          join rf in _context.RoleFunctions on ur.RoleId equals rf.RoleId
                                          join f in _context.Functions on rf.FunctionId equals f.Id
                                          where ur.UserId == userId && f.Code == _functionCode
                                          select f).AnyAsync();

            if (!hasPermission)
            {
                // Trả về 403 Forbidden nếu không có quyền
                context.Result = new ObjectResult("Bạn không có quyền thực hiện chức năng này!")
                {
                    StatusCode = 403
                };
            }
        }
    }
}