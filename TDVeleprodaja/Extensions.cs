using AR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDVeleprodaja.Models;

namespace TDVeleprodaja
{
    public static class Extensions
    {
        public static AR.WebShop.Cart GetCart(this HttpRequest Request)
        {
            if (!Request.IsLogged())
                return null;


            AR.WebShop.Cart c = AR.WebShop.Cart.Get(Request.Cookies["arCart"]);

            if (c == null)
            {
                string hash;
                c = AR.WebShop.Cart.Create(out hash);
                Request.HttpContext.Response.Cookies.Append("arCart", hash);
            }

            return c;
        }
        public static bool IsLogged(this HttpRequest Request)
        {
            string k = Request.Cookies.Where(t => t.Key == "h").FirstOrDefault().Value;
            return AR.ARWebAuthorization.IsLogged(k);
        }
        public static double GetDiscount(this AR.WebShop.Cart.Item k, int userID)
        {
            User<MoreInformationAboutUser> user = User<MoreInformationAboutUser>.GetUser(userID);
            PriceList pl = PriceList.GetPriceList(user.PriceListID);

            return pl.GetItem(k.ProductID).GetDiscount(k.Quantity);
        }
        public static double GetValueWithoutDiscount(this AR.WebShop.Cart cart, int userID)
        {
            double value = 0;

            List<AR.WebShop.Cart.Item> items = cart.GetItems();
            User<MoreInformationAboutUser> user = User<MoreInformationAboutUser>.GetUser(userID);

            PriceList pl = PriceList.GetPriceList(user.PriceListID);

            foreach (AR.WebShop.Cart.Item item in items)
                value += item.Quantity * pl.GetItem(item.ProductID).Price;

            return value;
        }
        public static double GetValueWithDiscount(this AR.WebShop.Cart cart, int userID)
        {
            double value = 0;

            List<AR.WebShop.Cart.Item> items = cart.GetItems();
            User<MoreInformationAboutUser> user = User<MoreInformationAboutUser>.GetUser(userID);

            PriceList pl = PriceList.GetPriceList(user.PriceListID);

            foreach (AR.WebShop.Cart.Item item in items)
            {
                PriceList.Item plItem = pl.GetItem(item.ProductID);
                value += item.Quantity * (plItem.Price + (plItem.Price * (plItem.GetDiscount(item.Quantity) / 100)));
            }

            return value;
        }
        public static double GetDiscountValue(this AR.WebShop.Cart cart, int userID)
        {
            double discountValue = 0;

            User<MoreInformationAboutUser> user = User<MoreInformationAboutUser>.GetUser(userID);
            PriceList pl = PriceList.GetPriceList(user.PriceListID);

            foreach (AR.WebShop.Cart.Item item in cart.GetItems())
            {
                double valueWithoutDiscount = item.Quantity * pl.GetItem(item.ProductID).Price;
                discountValue += valueWithoutDiscount + (valueWithoutDiscount * (pl.GetItem(item.ProductID).GetDiscount(item.Quantity) / 100));
            }

            return discountValue;
        }

        public static void FinishOrder(this AR.WebShop.Cart cart, AR.ARIAuthorizable user)
        {
            Order newOrder = new Order();
            newOrder.UserID = user.ID;
            newOrder.Date = DateTime.Now;
            newOrder.Status = OrderStatus.Obrada;

            User<MoreInformationAboutUser> us = user as User<MoreInformationAboutUser>;
            PriceList pl = PriceList.GetPriceList(us.PriceListID);
            newOrder.Items = new List<Order.Item>();

            foreach (var v in cart.GetItems())
                newOrder.Items.Add(new Order.Item()
                {
                    ProductID = v.ProductID,
                    Quantity = v.Quantity,
                    Price = pl.GetItem(v.ProductID).Price,
                    Discount = pl.GetItem(v.ProductID).GetDiscount(v.Quantity)
                });

            newOrder.Add();
            cart.Clear();
        }
        public static Models.User<Models.MoreInformationAboutUser> GetLocalUser(this HttpRequest Request)
        {
            AR.ARWebAuthorization.User u = Request.GetUser();

            if (u == null || u.LocalUserClass == null)
                return null;

            return u.LocalUserClass as Models.User<MoreInformationAboutUser>;
        }

        public static AR.ARWebAuthorization.User GetUser(this HttpRequest Request)
        {
            return AR.ARWebAuthorization.GetUser(Request.Cookies["h"]);
        }
        public static bool IsAdmin(this HttpRequest Reques)
        {
            var u = Reques.GetLocalUser();

            if (u == null)
                return false;

            if (u.Type == UserType.Administrator)
                return true;
            return false;
        }
    }
}
