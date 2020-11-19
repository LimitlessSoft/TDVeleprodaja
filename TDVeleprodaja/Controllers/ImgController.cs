using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TDVeleprodaja.Controllers
{
    public class ImgController : ARNetCoreGallery
    {
        public static IHostingEnvironment e;
        public ImgController(IHostingEnvironment ev) : base(ev, "img")
        {

        }
        
        
        [HttpPost]
        [Route("/Img/UploadImage")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            return await Task.Run<IActionResult>(() =>
            {
                string otp = "";
                try
                {
                    JsonResult t = ImageUpload(files).Result as JsonResult;
                    string[] fullPaths = t.Value as string[];

                    if(fullPaths != null)
                    {
                        for(int i = 0; i < fullPaths.Length; i++)
                        {
                            string k = fullPaths[i];
                            otp += fullPaths[i].Substring(fullPaths[i].IndexOf("\\img")).Replace("\\", "/");
                        }
                    }
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    otp = ex.Message;
                }
                return Json(otp);

            });
        }
    }
}
