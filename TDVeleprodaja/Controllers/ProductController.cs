using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AR.WebShop;
using ARNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Org.BouncyCastle.Asn1.Ocsp;
using TDVeleprodaja.Models;
using Ubiety.Dns.Core;
using Microsoft.AspNetCore.Hosting;


namespace TDVeleprodaja.Controllers
{
    public class ProductController : Controller
    {
        [Route("/Shop")]
        public IActionResult List()
        {
            if (!Request.IsLogged())
                return Redirect("/");


            return View(PriceList.GetPriceList(Request.GetLocalUser().PriceListID));
        }
        [Route("/Product/New")]
        public IActionResult New()
        {
            if (!Request.IsAdmin())
                return Redirect("/Home/Index");

            return View();
        }
        [Route("/Shop/Margin/{id}")]
        public IActionResult Index(int id)
        {
            Models.Product p = Models.Product.BufferedList().Where(t => t.ID == id).FirstOrDefault();

            if (p == null)
                return Json("Product not found!");

            return View(p);
        }
        public Task<IActionResult> Add(Product p)
        {
            return Task.Run<IActionResult>(() =>
            {
                try
                {
                    p.AddAsync().Wait();
                    return Json("1");
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json("Greska");
                }
            });
        }
        [Route("/Shop/UpdateName")]
        public Task<IActionResult> ProductUpdateName(int productID, string newName)
        {
            return Task.Run<IActionResult>(() =>
            {
                try
                {
                    if (!Request.IsAdmin())
                        return Json("Nisi admin");

                    Models.Product p = Models.Product.GetProduct(productID);

                    p.Name = newName;
                    p.Update();

                    return Json("Success");
                }catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json(ex.Message);
                }
        });
        }
        [Route("/Shop/UpdateUnit")]
        public Task<IActionResult> ProductUpdateUnit(int productID, string newUnit)
        {
            return Task.Run<IActionResult>(() =>  
            {
                try
                {
                    if(!Request.IsAdmin())
                        return Json("Nisi admin!");

                    Models.Product p = Models.Product.GetProduct(productID);

                    if (p == null)
                        return Json("Product not found!");

                    p.Unit = newUnit;
                    p.Update();

                    return Json("Success");

                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json(ex.Message);
                }
            });
        }
        [Route("/Shop/UpdateDescription")]
        public Task<IActionResult> ProductUpdateDescription(int productID, string newDescription)
        {
            return Task.Run<IActionResult>(() =>
            {
                try
                {
                    if (!Request.IsAdmin())
                        return Json("Nisi admin!");

                    Models.Product p = Models.Product.GetProduct(productID);

                    if (p == null)
                        return Json("Product not found!");

                    p.Description = newDescription;
                    p.Update();

                    return Json("Success");

                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json(ex.Message);
                }
            });
        }
        [Route("/Shop/UpdatePrice")]
        public Task<IActionResult> ProductUpdatePrice(int PriceListID, double newPrice)
        {
            return Task.Run<IActionResult>(() =>
            {
                try
                {
                    if (!Request.IsAdmin())
                        return Json("Nisi admin!");

                    PriceList.Item t = PriceList.Item.List().Where(t=>t.ID == PriceListID).FirstOrDefault();

                    if (t == null)
                        return Json("Product not found!");


                    t.Price = newPrice;
                    t.Update();

                    return Json("Success");

                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json(ex.Message);
                }
            });
        }
        [Route("/Shop/UpdateMargin")]
        public Task<IActionResult> ProductUpdateMargin(int PriceListID, List<ProductMargin> m)
        {
            return Task.Run<IActionResult>(() =>
            {
                try
                {
                    return Json("DSAD");
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return Json(ex.Message);
                }
            });
        }
       
        [Route("/Product/{id}")]
        public IActionResult Details(int id)
        {
            return View(Product.BufferedList().Where(t => t.ID == id).FirstOrDefault());
        }
    
    
    
    }
}
