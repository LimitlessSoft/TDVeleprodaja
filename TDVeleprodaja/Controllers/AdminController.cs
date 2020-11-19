using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TDVeleprodaja.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (!Request.IsAdmin())
                return Redirect("/Shop");

            return View();
        }
    }
}
