
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace TDVeleprodaja.Models
{

    public enum UserType
    {
        Administrator = 0,
        PravnoLice = 1,
        FizickoLice = 2
    }

    public class User<T> : AR.ARIAuthorizable
    {

        private static List<User<T>> _bufferedList = new List<User<T>>(); 
        public int ID { get; set; }
        public string Name { get; set; }
        public string PW { get; set; }
        public int PriceListID { get; set; }
        public UserType Type { get; set; }
        public T Tag { get; set; }


        static User()
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
        public User()
        {
        }

        private static Task _UpdateBufferAsync()
        {
            return Task.Run(() =>
            {
                _bufferedList = ListAsync().Result;
            });
        }

        public static Task<User<T>> ValidateAsync(string Username, string Password)
        {
            return Task.Run(() =>
            {
                if (_bufferedList.Count() == 0)
                    _UpdateBufferAsync().Wait();
                return _bufferedList.Where(t => t.Name == Username && t.PW == AR.Security.HashPW(Password)).FirstOrDefault();
            });
        }
        public static Task<List<User<T>>> ListAsync()
        {
            return Task.Run(() => {
                List<User<T>> list = new List<User<T>>();
                try
                {
                    using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand("SELECT ID, NAME, PASSWORD, PRICELISTID, TYPE, TAG FROM USER", con))
                        {
                            using (MySqlDataReader dt = cmd.ExecuteReader())
                            {
                                while (dt.Read())
                                {
                                    list.Add(new User<T>()
                                    {
                                        ID = Convert.ToInt32(dt["ID"]),
                                        Name = dt["NAME"].ToString(),
                                        Type = (UserType)Convert.ToUInt32(dt["TYPE"]),
                                        PriceListID = Convert.ToInt32(dt["PRICELISTID"]),
                                        PW = dt["PASSWORD"].ToString(),
                                        Tag = JsonConvert.DeserializeObject<T>(dt["TAG"].ToString())
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
                    return list;
                }
            });
        }
        public static List<User<T>> BufferedList()
        {
            if (_bufferedList == null)
               _UpdateBufferAsync().Wait();

            return _bufferedList;
        }
        public static User<T> GetUser(int ID)
        {
            return _bufferedList.Where(t => t.ID == ID).FirstOrDefault();
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
                if (this.ID != 0)
                    return;

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
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO USER(NAME, PASSWORD, PRICELISTID,TYPE, TAG) VALUES(@NAME, @PASSWORD, @PRICELISTID, @TYPE, @TAG)", con))
                    {
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@PASSWORD", AR.Security.HashPW(this.PW));
                        cmd.Parameters.AddWithValue("@TYPE", (int)this.Type);
                        cmd.Parameters.AddWithValue("@PRICELISTID", this.PriceListID);
                        cmd.Parameters.AddWithValue("@TAG", JsonConvert.SerializeObject(this.Tag));
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
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM USER WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", this.ID);
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
        public void Update()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE USER SET NAME=@NAME, TYPE=@TYPE, TAG=@TAG, PRICELISTID = @PRICELISTID WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@TYPE", (int)this.Type);
                        cmd.Parameters.AddWithValue("@PRICELISTID", this.PriceListID);
                        cmd.Parameters.AddWithValue("@TAG", JsonConvert.SerializeObject(this.Tag));
                    }
                }

                _UpdateBufferAsync();
            }
            catch (Exception ex)
            {
                AR.ARDebug.Log(ex.Message);
            }
        }
    }
}

