using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDVeleprodaja
{
    public static class Extensions
    {
        public static bool IsLogged(this HttpRequest Request)
        {
            string k = Request.Cookies.Where(t => t.Key == "h").FirstOrDefault().Value;
            return AR.ARWebAuthorization.IsLogged(k);
        }
    }
}
