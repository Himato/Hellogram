using System.Web.Mvc;

namespace HelloGram.Controllers
{
    [AllowAnonymous]
    public class ErrorsController : Controller
    {
        // GET: Error
        [Route("~/NotFound")]
        public ActionResult NotFound()
        {
            const int statusCode = (int) System.Net.HttpStatusCode.NotFound;

            Response.StatusCode             = statusCode;
            Response.TrySkipIisCustomErrors = true;

            HttpContext.Response.StatusCode             = statusCode;
            HttpContext.Response.TrySkipIisCustomErrors = true;

            return PartialView();
        }

        [Route("~/Error")]
        public ActionResult Error(string q = null)
        {
            Response.StatusCode             = (int) System.Net.HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;

            ViewBag.Message = q;

            return View();
        }
    }
}