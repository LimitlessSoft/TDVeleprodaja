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
            public string TransportingPackage { get; set; }

            public List<Margin> Marings { get; set; }

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
                if (_bufferedList == null)
                    _UpdateBufferAsync();
                   
                return _bufferedList;
            }
            private async static void _UpdateBufferAsync()
            {
                _bufferedList = await ListAsync();
            }
            public async static Task<List<PriceList.Item>> ListAsync()
            {
                return await Task.Run(() =>
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
                                            TransportingPackage = dt["TRANSPORTINGPACKAGE"].ToString(),
                                            Marings = JsonConvert.DeserializeObject<List<Margin>>(dt["MARGINS"].ToString())
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
                            cmd.Parameters.AddWithValue("@MARGINS", JsonConvert.SerializeObject(this.Marings));
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
                            cmd.Parameters.AddWithValue("@MARG", JsonConvert.SerializeObject(this.Marings));
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

            
        }
    }
}
