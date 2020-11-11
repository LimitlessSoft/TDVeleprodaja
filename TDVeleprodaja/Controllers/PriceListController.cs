using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using TDVeleprodaja.Models;

namespace TDVeleprodaja.Controllers
{
    public class PriceListController : Controller
    {
        [Route("/PriceList")]
        public IActionResult List()
        {
            return View(PriceList.BufferedList());
        }
        [Route("/PriceList/New")]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [Route("/PriceList/Add")]
        public async Task<IActionResult> Add(string name, int priceListType)
        {
            return await Task.Run(() =>
            {
                if (!Request.IsAdmin())
                    return Json("Nemate dovoljno prava!");

                try
                {
                    PriceList pl = new PriceList();
                    pl.Name = name;
                    pl.Type = (PriceListType)priceListType;
                    pl.Add();
                    return Json("1");
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json(ex.ToString());
                }
            });
        }
        [HttpPost]
        [Route("/PriceList/Update/Name")]
        public async Task<IActionResult> UpdateName(int id, string newName)
        {
            return await Task.Run(() =>
            {
                if (!Request.IsAdmin())
                    return Json("Nemate dovoljno prava!");

                try
                {
                    PriceList pl = PriceList.GetPriceList(id);
                    pl.Name = newName;
                    pl.Update();
                    return Json("1");
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json(ex.ToString());
                }
            });
        }

    }
}
