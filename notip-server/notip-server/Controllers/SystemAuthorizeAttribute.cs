using Microsoft.AspNetCore.Mvc;

namespace notip_server.Controllers
{

    public class SystemAuthorizeAttribute : TypeFilterAttribute
    {
        public SystemAuthorizeAttribute() : base(typeof(SystemAuthorizeAttribute)) { }
    }
}
