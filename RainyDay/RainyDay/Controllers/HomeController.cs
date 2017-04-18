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

        [HttpGet]
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
            Data model = new Data();
                
            model.keyword_input = "Keyword input : " + HttpContext.Request.Query["keyword"];
            model.algorithm = "Algorithm : " + HttpContext.Request.Query["algorithm"];
            
            return View(model);
        }
    }
}
