namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub.Controllers
{
    using System.Web.Http;
    using System.Web.Http.Results;
    using Models;

    public class HomeController : ApiController
    {
        public JsonResult<HomeModel> Get()
        {
            return Json(new HomeModel
            {
                Name = "Particular ServiceControl Stub",
                Description = "This is a Particular ServiceControl stub interface for testing purpose."
            });
        }
    }
}