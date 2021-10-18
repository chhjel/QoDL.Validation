using Microsoft.AspNetCore.Mvc;
using QoDL.DataAnnotations.Extensions.Extensions;
using System;

namespace DevTesting.NetCore.Controllers
{
    public class HomeController : Controller
    {
        private static Random _rand = new Random();

        public IActionResult Index()
        {
            if (!ModelState.IsValid)
            {
                // Create a result from modelstate, containing any errors.
                return ModelState.CreateJsonResult();
            }

            if (_rand.Next(100) > 50)
            {
                return ModelState.CreateJsonResult("Some error");
            }

            return ModelState.CreateJsonResult();
        }
    }
}
