using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            if (Request.IsLogged())
                return Redirect("/Shop");
            return View();
        }

        [Route("/Registracija")]
        public IActionResult Registration()
        {
            return View(new User<MoreInformationAboutUser>());
        }
        public IActionResult RegistrationUser(User<MoreInformationAboutUser> t)
        {
            return View();
        }
        #region API
        [Route("/Login")]
        public async Task<IActionResult> Login(string Username, string Passwrod)
        {
            return await Task.Run(() =>
            {
                User<MoreInformationAboutUser> us = User<MoreInformationAboutUser>.ValidateAsync(Username, Passwrod).Result;
                if(us != null)
                {
                    string tk;
                    AR.ARWebAuthorization.LogUser(us, out tk);
                    Response.Cookies.Append("h", tk);

                    return Json("1");
                }
                
                return Json("Pogresan username ili password");
            });
           
        }
        #endregion

    }
}
