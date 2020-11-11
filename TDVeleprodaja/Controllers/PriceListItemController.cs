using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TDVeleprodaja.Controllers
{
    public class PriceListItemController : Controller
    {


        public IActionResult New()
        {
            if (!Request.IsAdmin())
                return Redirect("/Home/Index");

            return View();
        }
        [Route("/PriceListItem/Margin/Add")]
        public async Task<IActionResult> MarginAdd(int ID, double transportingPackages, double discount)
        {
            return await Task.Run<IActionResult>(() =>
            {
                if (!Request.IsAdmin())
                    return Json("Nemate dovoljno prava!");

                try
                {

                    Models.PriceList.Item item = Models.PriceList.Item.BufferedList().Where(x => x.ID == ID).FirstOrDefault();

                    if (item == null)
                        return Json("Item not found!");

                    if (item.Margins == null)
                        item.Margins = new List<Models.ProductMargin>();

                    item.Margins.Add(new Models.ProductMargin()
                    {
                        TransportingPackages = transportingPackages,
                        Discount = discount
                    });
                    item.Update();
                    return Json("1");
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.ToString());
                    return Json(ex.ToString());
                }
            });
        }
    }
}
