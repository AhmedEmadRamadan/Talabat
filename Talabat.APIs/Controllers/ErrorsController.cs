using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    [Route("errors/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error (int code)
        {
            return NotFound(new ApiResponse(code));
        }
    }
}
