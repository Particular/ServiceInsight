using System.Web.Http;
using System.Web.Http.Results;

namespace NServiceBus.Profiler.FunctionalTests.ServiceControlStub.Controllers
{
    public class HomeController : ApiController
    {
        public JsonResult<HomeModel> Get()
        {
            return Json(new HomeModel
            {
                Name = "Particular Management Stub",
                Description = "This is a Particular Management stub interface for testing purpose."
            });
        }
    }
}