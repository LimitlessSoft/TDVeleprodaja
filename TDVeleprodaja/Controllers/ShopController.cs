using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR.WebShop;
using ARNetCore.ARWebAuthorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class ShopController : Controller
    {
        [Route("/Shop")]
        public IActionResult Index()
        {
            if(Request.IsLogged())
                return View(Models.Product<MoreInformationAboutProduct>.Buffer());
            AR.WebShop.Cart
            return Redirect("/Home/Index");
        }

        [Route("/Product/{id}")]
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
