using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class OrderController : Controller
    {
        //Retrun order list
        public IActionResult Index()
        {
            if (!Request.IsLogged())
                return Redirect("/Home/Index");
            
            if(Request.IsAdmin())
                return View(Order.BuffredList());

            User<MoreInformationAboutUser> t = Request.GetLocalUser();
            return View(Order.BuffredList().Where(t=>t.UserID == t.UserID).ToList());
        }

        [Route("/Order/Close")]
        public IActionResult CloseOrder()
        {
            AR.WebShop.Cart cart = Request.GetCart();
            User<MoreInformationAboutUser> user = Request.GetLocalUser();
            
            return View();
        }
        [Route("/Order/{id}")]
        public IActionResult Details(int id)
        {
            return View(Order.GetOrder(id));
        }


    }
}
