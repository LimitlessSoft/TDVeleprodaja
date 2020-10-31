﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public enum PriceListType
    {
        Silver = 0,
        Gold = 1
    }
    public partial class PriceList
    {
        private static List<PriceList> _bufferedList = new List<PriceList>();


        public int ID { get; set; }
        public string Name { get; set; }
        public PriceListType Type { get; set; }


        static PriceList()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    _UpdateBufferAsync().Wait();
                    Thread.Sleep(1000 * 60 * 60);
                }
            });
        }
        public PriceList()
        {


            User<object> u = new User<object>();

            string hash;
            AR.ARWebAuthorization.LogUser(u, out hash);
            AR.ARWebAuthorization.UserTimeoutTime = 10;
            // Resposnse.Cookies.Append("h", hash);

            




            AR.ARWebAuthorization.User user = AR.ARWebAuthorization.GetUser(hash);

            if (user != null)
                user.UpdateAliveStatus();

        }


        private static Task _UpdateBufferAsync()
        {
            return Task.Run(() => 
            {
                _bufferedList = ListAsync().Result;
            });
        }


        public static List<PriceList> BufferedList()
        {
            if (_bufferedList == null)
                _bufferedList = ListAsync().Result;

            return _bufferedList;
        }
        public static PriceList GetPriceList(int ID)
        {
            return _bufferedList.Where(t => t.ID == ID).FirstOrDefault();
        }
        public static Task<List<PriceList>> ListAsync()
        {
            return Task.Run(() =>
            {
                List<PriceList> list = new List<PriceList>();
                try
                {
                    using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using(MySqlCommand cmd = new MySqlCommand("SELECT ID, NAME, TYPE FROM PRICELIST", con))
                        {
                            using(MySqlDataReader dt = cmd.ExecuteReader())
                            {
                                while (dt.Read())
                                {
                                    list.Add(new PriceList()
                                    {
                                        ID = Convert.ToInt32(dt["ID"]),
                                        Name = dt["NAME"].ToString(),
                                        Type = (PriceListType)Convert.ToInt32(dt["TYPE"].ToString())
                                    });
                                }
                            }
                        }
                    }
                    return list;
                }
                catch(Exception ex)
                {
                    AR.ARDebug.Log(ex.Message);
                    return new List<PriceList>();
                }
                
            });
        }

        public void Add()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO PRICELIST(NAME, TYPE) VALUES(@NAME, @TYPE)", con))
                    {
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@TYPE", (int)this.Type);
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
        public void Remove()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM PRICELIST WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", this.ID);
                        cmd.ExecuteNonQuery();
                    }
                    _UpdateBufferAsync();
                }
            }
            catch (Exception ex)
            {
                AR.ARDebug.Log(ex.Message);
            }
        }
        public void Update()
        {
            using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE PRICELIST SET NAME = @NAME, TYPE = @TYPE WHERE ID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@NAME", this.Name);
                    cmd.Parameters.AddWithValue("@TYPE", (int)this.Type);
                    cmd.Parameters.AddWithValue("@ID", this.ID);
                    cmd.ExecuteNonQuery();
                    _UpdateBufferAsync();
                }
            }
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
                Update();
            });
        }
    }
}