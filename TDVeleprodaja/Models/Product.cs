using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TDVeleprodaja.Models
{
    public class Product
    {
        private static List<Product> _bufferedList = new List<Product>();
        public int ID { get; set; }
        public string CatalogueID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public Object Tag { get; set; }

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
        public Product()
        {

        }


        public static Product GetProduct(int id)
        {
            return _bufferedList.Where(t => t.ID == id).FirstOrDefault();
        }
        private static Task __UpdateBuffer()
        {
            return Task.Run(() =>
            {
                _bufferedList = List().Result;
            });
            
        }
        public static List<Product> BufferedList()
        {
            if (_bufferedList == null || _bufferedList.Count == 0)
                __UpdateBuffer().Wait();

           return _bufferedList;
        }
        public async static Task<List<Product>> List()
        {
            return await Task.Run(() =>
            {
                List<Product> list = new List<Product>();
                try
                {
                    using(MySqlConnection con = new MySqlConnection(Program.ConnectionString))
                    {
                        con.Open();
                        using(MySqlCommand cmd = new MySqlCommand("SELECT ID, CATALOGUEID, NAME, UNIT, THUMBNAIL, DESCRIPTION, TAG FROM PRODUCT", con))
                        {
                            using(MySqlDataReader dt = cmd.ExecuteReader())
                            {
                                while (dt.Read())
                                {
                                    list.Add(new Product()
                                    {
                                        ID = Convert.ToInt32(dt["ID"].ToString()),
                                        CatalogueID = dt["CATALOGUEID"].ToString(),
                                        Name = dt["NAME"].ToString(),
                                        Unit = dt["UNIT"].ToString(),
                                        Thumbnail = dt["THUMBNAIL"].ToString(),
                                        Description = dt["DESCRIPTION"].ToString(),
                                        Tag = JsonConvert.DeserializeObject(dt["TAG"].ToString())

                                    }); ;
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    list = new List<Product>();
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
                    using (MySqlCommand cmd = new MySqlCommand("INSERT INTO PRODUCT(CATALOGUEID ,NAME ,THUMBNAIL ,UNIT, DESCRIPTION, TAG) VALUES(@CATALOGUEID,@NAME, @THUMBNAIL, @UNIT, @DESCRIPTION, @TAG)", con))
                    {
                        cmd.Parameters.AddWithValue("@CATALOGUEID", this.CatalogueID);
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@UNIT", this.Unit);
                        cmd.Parameters.AddWithValue("@THUMBNAIL", this.Thumbnail);
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
                using(MySqlCommand cmd = new MySqlCommand("UPDATE PRODUCT SET CATALOGUEID = @CATALOGUEID, NAME = @NAME, UNIT = @UNIT, DESCRIPTION = @DESC, TAG = @TAG WHERE ID = @ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", this.ID);
                    cmd.Parameters.AddWithValue("@STOCKNUMBER", this.CatalogueID);
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
