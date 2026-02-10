using Microsoft.AspNetCore.Mvc;

namespace YC5.Filters
{
    public class CheckPermissionAttribute : TypeFilterAttribute
    {
        public CheckPermissionAttribute(string functionCode) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { functionCode };
        }
    }
}