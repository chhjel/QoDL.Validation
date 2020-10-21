using DevTesting.Models;
using QoDL.DataAnnotations.LibraryValidation.Extensions;
using QoDL.DataAnnotations.Security.GhostField;
using QoDL.DataAnnotations.Security.SessionToken;
using System.Web.Mvc;

namespace DevTesting.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new TestModel());
        }

        [HttpPost]
        [ValidateGhostField]
        [ValidateSessionToken]
        public ActionResult Submit(TestModel model)
        {
            return View("Index", new TestModel());
        }

        [HttpGet]
        [ValidateSessionToken]
        public ActionResult ApiTest(TestModel model)
        {
            return ModelState.CreateJsonResult();
        }

        [HttpGet]
        public ActionResult ApiTest2(TestModel model)
        {
            return ModelState.CreateJsonResult(flags: new []{ "flag_test" });
        }
    }
}