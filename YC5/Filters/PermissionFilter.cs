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
            // 1. Kiểm tra xem user đã đăng nhập chưa
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // 2. Lấy danh sách các "Permission" đã được dán trong Token
            var userPermissions = context.HttpContext.User.FindAll("Permission").Select(x => x.Value);

            // 3. So khớp với mã chức năng yêu cầu (ví dụ: STUDENT_VIEW)
            if (!userPermissions.Contains(_functionCode))
            {
                context.Result = new ObjectResult("Bạn không có quyền thực hiện chức năng này!")
                {
                    StatusCode = 403
                };
            }
        }
    }
}