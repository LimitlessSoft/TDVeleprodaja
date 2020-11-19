using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TDVeleprodaja.Controllers
{
    public class CartController : Controller
    {
        [Route("/Cart")]
        public IActionResult Index()
        {
            AR.WebShop.Cart k = Request.GetCart();

            if (k == null || k.GetItems().Count() == 0)
                return View("Error", "Vasa korpa je prazan!");

            return View(k);
        }

        [HttpPost]
        [Route("/Cart/Add")]
        public async Task<IActionResult> Add(int productID, double quantity)
        {
            return await Task.Run(() =>
            {
                if (!Request.IsLogged())
                    return Json("Niste logovani!");

                try
                {
                    Request.GetCart().AddItem(productID, quantity);
                    return Json("1");
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json("Doslo je do greske prilikom dodavanja proizvoda u korpu!");
                }
            });
        }
        [HttpPost]
        [Route("/Cart/Remove")]
        public async Task<IActionResult> Remove(int productID)
        {
            return await Task.Run(() =>
            {
                if (!Request.IsLogged())
                    return Json("Niste logovani!");

                try
                {
                    Request.GetCart().RemoveItem(productID);
                    return Json("1");
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json("Doslo je do greske prilikom dodavanja proizvoda u korpu!");
                }
            });
        }
        [HttpPost]
        [Route("/Cart/Quantity/Update")]
        public async Task<IActionResult> QuantityUpdate(int productID, double newQuantity)
        {
            return await Task.Run(() =>
            {
                if (!Request.IsLogged())
                    return Json("Niste logovani!");

                try
                {
                    Request.GetCart().SetQuantity(productID, newQuantity);
                    return Json("1");
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json("Doslo je do greske prilikom dodavanja proizvoda u korpu!");
                }
            });
        }

        [Route("/Cart/Conclude")]
        public async Task<IActionResult> ConcludeOrder()
        {
            return await Task.Run<IActionResult>(() =>
            {
                try
                {
                    Request.GetCart().FinishOrder(Request.GetLocalUser());
                    return Json("1");
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json("Doslo je do greske prilikom zakljucivanja porudzbine!");
                }
            });
        }
    }
}
