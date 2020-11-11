using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class User : Controller
    {
        [Route("/User/List")]
        public IActionResult List()
        {
            if (!Request.IsAdmin())
                return Redirect("/Home/Index");

            return View(User<MoreInformationAboutUser>.BufferedList());
        }
        [Route("/User/New")]
        public IActionResult New()
        {
            if (!Request.IsAdmin())
                return Redirect("/Home/Index");

            return View();

        }
    }
}
