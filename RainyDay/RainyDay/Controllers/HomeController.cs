using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RainyDay.Models;

namespace RainyDay.Controllers
{
    public class HomeController : Controller
    {
        /* View page only (for navigation bar) */

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        
        public IActionResult Search()
        {
            Data model = new Data()
            {
                keyword_input = "Keyword input : " + HttpContext.Request.Query["keyword"],
                algorithm = "Algorithm : " + HttpContext.Request.Query["algorithm"]
            };

            return View(model);
        }
        
        [HttpPost]
        public IActionResult HandleAjax(string query) // event handler for ajax HTTP request, data value stored in query
        {
            return Content("You typed : " + query); // sent response as string using ContentResult
        }
    }
}