using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NorthWindRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestiController : ControllerBase
    {

        [HttpGet]
        public ActionResult GetData()
        {
            return Ok("Yet another Hello World Demo!");
        }

    }
}
