using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using notip_server.Middlewares;

namespace notip_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ModeratorMiddleware))]
    public class StaffController : ControllerBase
    {
    }
}
