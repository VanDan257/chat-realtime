using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace notip_server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "PNChat API already";
        }
    }
}
