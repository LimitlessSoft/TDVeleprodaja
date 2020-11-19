using AR.WebShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public partial class Order

    {
        public class Item
        {
            public int ProductID { get; set; }
            public double Quantity { get; set; }
            public double Price { get; set; }
            /// <summary>
            /// Procentuali rabat
            /// </summary>
            public double Discount { get; set; }

            public void ADS()
            {
                AR.WebShop.Cart c = new Cart();
            }
        }

        // Order.TotalDiscount
        // Order.GetTotalDiscount // popust cele porudzbine

        // Cart.Item.GetDiscount 

        // item1 - 1 - 10
        // item2 - 2 - 20
    }
}
