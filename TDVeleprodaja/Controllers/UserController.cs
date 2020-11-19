using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class UserController : Controller
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
        [Route("/User/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            return View(User<MoreInformationAboutUser>.BufferedList().Where(t => t.ID == id).FirstOrDefault());
        }
        [HttpPost]
        [Route("/User/Update")]
        public IActionResult Update(User<MoreInformationAboutUser> us)
        {
            try
            {
                us.Update();
                return Json("1");
            }
            catch(Exception ex)
            {
                AR.ARDebug.Log(ex.Message);
                return Json("Doslo je do greske!");
            }
        }
    }
}
