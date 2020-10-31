using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public class Product<T>
    {
        private static List<Product<T>> _bufferedList = new List<Product<T>>();
        public int ID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public T Tag { get; set; }

        static Product()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    __UpdateBuffer();
                    Thread.Sleep(1000 * 60 * 60);
                }
            });
        }
        public Product(int ID)
        {
            _bufferedList.Where(t => t.ID == ID).FirstOrDefault();
        }
        public Product()
        {

        }


        private static Task __UpdateBuffer()
        {
            return Task.Run(() =>
            {
                _bufferedList = List().Result;
            });
            
        }
        public static List<Product<T>> Buffer()
        {
            if (_bufferedList == null || _bufferedList.Count == 0)
                __UpdateBuffer().Wait();

           return _bufferedList;
        }
        public async static Task<List<Product<T>>> List()
        {
            return await Task.Run(() =>
            {
                List<Product<T>> list = new List<Product<T>>();
                try
                {
                    using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using(MySqlCommand cmd = new MySqlCommand("SELECT ID, NAME, UNIT, THUMBNAIL, DESCRIPTION, TAG FROM PRODUCT", con))
                        {
                            using(MySqlDataReader dt = cmd.ExecuteReader())
                            {
                                while (dt.Read())
                                {
                                    list.Add(new Product<T>()
                                    {
                                        ID = Convert.ToInt32(dt["ID"].ToString()),
                                        Name = dt["NAME"].ToString(),
                                        Unit = dt["UNIT"].ToString(),
                                        Thumbnail = dt["THUMBNAIL"].ToString(),
                                        Description = dt["DESCRIPTION"].ToString(),
                                        Tag = JsonConvert.DeserializeObject<T>(dt["TAG"].ToString())

                                    });
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    list = new List<Product<T>>();
                    AR.ARDebug.Log(ex.Message);

                }
                return list;
            });
        }

        //DODATI SLIKU
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


        public void Add()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO PRODUCT(NAME, UNIT, DESCRIPTION, TAG) VALUES(@NAME, @UNIT, @DESCRIPTION, @TAG)", con))
                    {
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@UNIT", this.Unit);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", this.Description);
                        cmd.Parameters.AddWithValue("@TAG", JsonConvert.SerializeObject(this.Tag));
                        cmd.ExecuteNonQuery();
                        Task.Run(() =>
                        {
                            __UpdateBuffer();
                        });
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
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM PRODUCT WHERE ID = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", this.ID);
                        cmd.ExecuteNonQuery();
                        Task.Run(() =>
                        {
                            __UpdateBuffer();
                        });
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
            using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
            {
                con.Open();
                using(MySqlCommand cmd = new MySqlCommand("UPDATE PRODUCT SET NAME = @NAME, UNIT = @UNIT, DESCRIPTION = @DESC, TAG = @TAG WHERE ID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", this.ID);
                    cmd.Parameters.AddWithValue("@NAME", this.Name);
                    cmd.Parameters.AddWithValue("@UNIT", this.Unit);
                    cmd.Parameters.AddWithValue("@DESC", this.Description);
                    cmd.Parameters.AddWithValue("@TAG", JsonConvert.SerializeObject(this.Tag));
                    cmd.ExecuteNonQuery();
                    Task.Run(() => { __UpdateBuffer(); });
                }
            }
        }
    }
}
