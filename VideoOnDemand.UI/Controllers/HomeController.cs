using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using VideoOnDemand.UI.Models;
using VideoOnDemand.Data.Data.Entities;
using VideoOnDemand.UI.Repositories;

namespace VideoOnDemand.UI.Controllers
{
    public class HomeController : Controller
    {
        private SignInManager<User> _signInManager;

        public HomeController(SignInManager<User> signInMgr)
        {
            _signInManager = signInMgr;
        }

        // login view only should be displayed to visitors who haven’t already logged in,
        // Check if user is logged in
        public IActionResult Index()
        {
            //var rep = new MockReadRepository();
            //var courses = rep.GetCourses("4ad684f8 - bb70 - 4968 - 85f8 - 458aa7dc19a3");
            //var course = rep.GetCourse("4ad684f8-bb70-4968-85f8-458aa7dc19a3", 1);

            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
