using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public partial class PriceList
    {
        public class Item
        {

            private static List<PriceList.Item> _bufferedList = new List<PriceList.Item>();
            public int ID { get; set; }
            public int PriceListID { get; set; }
            public int ProductID { get; set; }
            public double Price { get; set; }
            public double TransportingPackage { get; set; }

            public List<ProductMargin> Margins { get; set; } = new List<ProductMargin>();

            static Item()
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        _UpdateBufferAsync();
                        Thread.Sleep(1000 * 60 * 60);
                    }
                });
            }

            public static List<PriceList.Item> BufferedList()
            {
                if (_bufferedList == null || _bufferedList.Count == 0)
                    _bufferedList = List();
                   
                return _bufferedList;
            }
            public static List<PriceList.Item> BufferedList(int PriceListID)
            {
                return BufferedList().Where(x => x.PriceListID == PriceListID).ToList();
            }
            private static void _UpdateBufferAsync()
            {
                Task.Run(() =>
                {
                    _bufferedList = ListAsync().Result;
                });
                
            }
            public static List<PriceList.Item> List()
            {
                List<PriceList.Item> list = new List<PriceList.Item>();
                try
                {
                    using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("SELECT ID, PRICELISTID, PRODUCTID, PRICE, TRANSPORTINGPACKAGE, MARGINS FROM PRICELIST_ITEM", con))
                        {
                            using (MySqlDataReader dt = cmd.ExecuteReader())
                            {
                                while (dt.Read())
                                {
                                    list.Add(new PriceList.Item()
                                    {
                                        ID = Convert.ToInt32(dt["ID"]),
                                        PriceListID = Convert.ToInt32(dt["PRICELISTID"].ToString()),
                                        ProductID = Convert.ToInt32(dt["PRODUCTID"].ToString()),
                                        Price = Convert.ToDouble(dt["PRICE"].ToString()),
                                        TransportingPackage =Convert.ToDouble(dt["TRANSPORTINGPACKAGE"]),
                                        Margins = JsonConvert.DeserializeObject<List<ProductMargin>>(dt["MARGINS"].ToString())
                                    });
                                }
                            }
                        }
                    }
                    return list;
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return new List<PriceList.Item>();
                }
            }
            public static List<PriceList.Item> List(int PriceListID)
            {
                return List().Where(x => x.PriceListID == PriceListID).ToList();
            }
            public static Task<List<PriceList.Item>> ListAsync()
            {
                return Task.Run(() =>
                {
                    return List();
                });
            }
            public static Task<List<PriceList.Item>> ListAsync(int PriceListID)
            {
                return Task.Run(() =>
                {
                    return List().Where(x => x.PriceListID == PriceListID).ToList();
                });
            }


            public Task UpdateAsync()
            {
                return Task.Run(() =>
                {
                    Update();
                });
            }
            public Task RemoveAsync()
            {
                return Task.Run(() =>
                {
                    Remove();
                });
            }
            public Task AddAsync()
            {
                return Task.Run(() =>
                {
                    Add();
                });
            }


            public void Add()
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO PRICELIST_ITEM(PRICELISTID, PRODUCTID, PRICE, TRANSPORTINGPACKAGE, MARGINS) VALUES(@PRI, @PRODU, @PRICE, @TRANSPORT, @MARGINS)", con))
                        {
                            cmd.Parameters.AddWithValue("@PRI", this.PriceListID);
                            cmd.Parameters.AddWithValue("@PRODU", this.ProductID);
                            cmd.Parameters.AddWithValue("@PRICE", this.Price);
                            cmd.Parameters.AddWithValue("@TRANSPORT", this.TransportingPackage);
                            cmd.Parameters.AddWithValue("@MARGINS", JsonConvert.SerializeObject(this.Margins));
                            cmd.ExecuteNonQuery();
                            _UpdateBufferAsync();
                        }
                    }
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                }
            }
            public void Remove()
            {
                try
                {
                    using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using(MySqlCommand cmd = new MySqlCommand("DELETE FROM PRICELIST_ITEM WHERE ID = @ID", con))
                        {
                            cmd.Parameters.AddWithValue("@ID", this.ID);
                            cmd.ExecuteNonQuery();
                            _UpdateBufferAsync();
                        }
                    }
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                }
            }
            public void Update()
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("UPDATE PRICELIST_ITEM SET PRICELISTID = @PRID, PRODUCTID = @PRDID, PRICE = @PRICE, TRANSPORTINGPACKAGE = @TRANSP, MARGINS = @MARG WHERE ID = @ID", con))
                        {
                            cmd.Parameters.AddWithValue("@ID", this.ID);
                            cmd.Parameters.AddWithValue("@PRID", this.PriceListID);
                            cmd.Parameters.AddWithValue("@PRDID", this.ProductID);
                            cmd.Parameters.AddWithValue("@PRICE", this.Price);
                            cmd.Parameters.AddWithValue("@TRANSP", this.TransportingPackage);
                            cmd.Parameters.AddWithValue("@MARG", JsonConvert.SerializeObject(this.Margins));
                            cmd.ExecuteNonQuery();
                            _UpdateBufferAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                }
            }

            /// <summary>
            /// Returns discount from margins based on input quantity
            /// </summary>
            /// <param name="Quantity"></param>
            /// <returns></returns>
            public double GetDiscount(double InputQuantity)
            {
                if (Margins == null)
                    return 0;

                double lastDiscount = 0;

                foreach (ProductMargin margin in Margins.OrderBy(x => x.TransportingPackages))
                    if (InputQuantity > margin.TransportingPackages)
                        lastDiscount = margin.Discount;
                    else
                        break;

                return lastDiscount;
            }
        }
    }
}
