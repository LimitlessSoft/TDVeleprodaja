using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Renci.SshNet.Messages.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public enum OrderStatus
    {
       Obrada = 0,
       CekaUplatu = 1,
       Uplacena = 2,
       Razvoz = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class Order
    {
        private static List<Order> _bufferedList = new List<Order>();


        public int ID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public List<Item> Items { get; set; }

        static Order()
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

        private static void _UpdateBuffer()
        {
            _bufferedList = List();
        }
        private static Task _UpdateBufferAsync()
        {
            return Task.Run(() =>
            {
                _UpdateBuffer();
            });
        }


        /// <summary>
        /// Returns list of orders from database
        /// </summary>
        /// <returns></returns>
        public static List<Order> List()
        {
            try
            {
                List<Order> list = new List<Order>();
                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT ID, USERID, DATE, STATUS, ITEMS  FROM ORDER_VP", con))
                    {
                        using (MySqlDataReader dt = cmd.ExecuteReader())
                        {
                            while (dt.Read())
                            {
                                list.Add(new Order()
                                {
                                    ID = Convert.ToInt32(dt["ID"]),
                                    UserID = Convert.ToInt32(dt["USERID"]),
                                    Date = Convert.ToDateTime(dt["DATE"].ToString()),
                                    Status = (OrderStatus)dt["STATUS"],
                                    Items = JsonConvert.DeserializeObject<List<Item>>(dt["ITEMS"].ToString())
                                });
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                AR.ARDebug.Log(ex.Message);
                return new List<Order>();
            }
        }
        /// <summary>
        /// Returns list of orders from database asynchronous
        /// </summary>
        /// <returns></returns>
        public static Task<List<Order>> ListAsync()
        {
            return Task.Run(() =>
            {
                return List();
            });
        }
        /// <summary>
        /// Returns list of orders from buffer
        /// </summary>
        /// <returns></returns>
        public static List<Order> BuffredList()
        {
            if (_bufferedList == null || _bufferedList.Count == 0)
                _UpdateBuffer();

            return _bufferedList;
        }
        /// <summary>
        /// Returns order.
        /// If order doesn't exists it will return null
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Order GetOrder(int ID)
        {
            if (_bufferedList == null || _bufferedList.Count == 0)
                _UpdateBuffer();

            return _bufferedList.Where(t => t.ID == ID).FirstOrDefault();
        }
         
        public Task AddAsync()
        {
            return Task.Run(() => 
            {
                Add();
            });
        }
        public Task RemoveAsync()
        {
            return Task.Run(() =>
            {
                Remove();
            });
        }
        public Task UpdateAsync()
        {
            return Task.Run(() =>
            {
                UpdateAsync();
            });
        }

        public void Add()
        {
            try
            {
                using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using(MySqlCommand cmd = new MySqlCommand("INSERT INTO ORDER_VP(USERID, DATE, STATUS, DISCOUNT, PRICE, ITEMS) VALUES(@USERID, @DATE, @STATUS, @DISCOUNT, @PRICE, @ITEMS)", con))
                    {
                        cmd.Parameters.AddWithValue("@USERID", this.UserID);
                        cmd.Parameters.AddWithValue("@DATE", this.Date);
                        cmd.Parameters.AddWithValue("@STATUS", (int)this.Status);
                        cmd.Parameters.AddWithValue("@ITEMS", JsonConvert.SerializeObject(this.Items));
                        cmd.Parameters.AddWithValue("@DISCOUNT", this.GetDiscountValue());
                        cmd.Parameters.AddWithValue("@PRICE", this.GetValueWithDiscount());
                        cmd.ExecuteNonQuery();
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
                using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE ORDER_VP SET PRICE = @PRICE, DESCRIPTION = @DESC WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", this.ID);
                        cmd.Parameters.AddWithValue("@DESC", JsonConvert.SerializeObject(this.Items));
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
                    using(MySqlCommand cmd = new MySqlCommand("DELETE FROM ORDER_VP WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", this.ID);
                        cmd.ExecuteReader();
                    }
                }
            }
            catch(Exception ex)
            {
                AR.ARDebug.Log(ex.Message);
            }
        }

        public double GetValue()
        {
            double tot = 0;
            foreach (Item i in Items)
                tot += i.Quantity * i.Price;
            return tot;
        }
        public double GetValueWithDiscount()
        {
            double tot = 0;
            foreach (Item i in Items)
                tot += i.Quantity * (i.Price + (i.Price * (i.Discount / 100)));
            return tot;
        }
        public double GetDiscountValue()
        {
            double tot = 0;
            foreach (Item i in Items)
                tot += i.Quantity * (i.Price * (i.Discount / 100));
            return tot;
        }
    }
}
