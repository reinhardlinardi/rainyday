using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RainyDay.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

        public IActionResult about()
        {
            return View();
        }
        
        public IActionResult search()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }
    }
}
