using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public class Cart
    {
        List<Cart_item> Items { get; set; }

        public void Add(Product<MoreInformationAboutProduct> pr)
        {
            Cart_item t = Items.Where(t => t.Product.ID == pr.ID).FirstOrDefault();
            if (t != null)
                t.RiseQuantity();
            else
                Items.Add(new Cart_item()
                {
                    Product = pr,
                    Quantity = 1
                }
                ); 


        }
        public class Cart_item
        {
            public Product<MoreInformationAboutProduct> Product { get; set; }
            public double Quantity { get; set; }

            public void RiseQuantity()
            {
                Quantity++;
            }

        }
        
    }

    
}
