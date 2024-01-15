using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Repo.Data.Contexts;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : APIBaseController
    {
        private readonly StoreContext _context;

        public BuggyController(StoreContext context)
        {
            _context = context;
        }
        //[HttpGet("NotFound")]
        //public ActionResult GetNotFound()
        //{

        //}
    }
}
