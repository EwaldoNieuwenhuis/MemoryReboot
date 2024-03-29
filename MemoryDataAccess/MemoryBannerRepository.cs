using BusinessMemory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;


namespace MemoryDataAccess
{
    public class MemoryBannerRepository : IMemoryItemRepository<byte[]>
    {
        public string ConnectionString { get; set; }

        private const string DefaultConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Ewaldo;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

        public MemoryBannerRepository()
        {
            ConnectionString = DefaultConnectionString;
        }

        public MemoryBannerRepository(string connectionstring)
        {
            ConnectionString = connectionstring;
        }

        public void Create(byte[] item, out int rowsAffected, out byte[] newRecord)
        {
            string sql = "INSERT INTO Banner (Banner) VALUES (@ImageData);";
            int id = 0;
            newRecord = item;

            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ImageData", SqlDbType.VarBinary, -1).Value = item;

                    connection.Open();
                    // Execute the command to insert the record and retrieve the generated Id
                    object result = command.ExecuteScalar();
                    // Check if the result is not null and convert it to the generated Id
                    if (result != null)
                    {
                        id = Convert.ToInt32(result);
                        rowsAffected = 1;
                    }
                    else
                    {
                        rowsAffected = 0;
                    }
                    connection.Close();
                }
            }
            

        }


        public void Delete(int id, out int rowsAffected)
        {
            throw new NotImplementedException();
        }

        public byte[] Read(int id)
        {
            throw new NotImplementedException();
        }

        public byte[] Read(byte[] image)
        {
            string sql = "SELECT * FROM Banner WHERE Banner = @ImageData;";
            byte[]? banner = null;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ImageData", image);

                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Haal de afbeeldingsgegevens op als byte array
                            banner = (byte[])reader["ImageData"];
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fout bij het lezen van de afbeelding uit de database: " + ex.Message);
                    // Handel de uitzondering af zoals nodig
                }
            }

            return banner;
        }

        public void Update(int id, byte[] item, out int rowsAffected, out byte[] updatedRecord)
        {
            throw new NotImplementedException();
        }

        public List<byte[]> ReadAll()
        {
            string sql = "SELECT * FROM Banner";
            List<byte[]> bannerDataList = new List<byte[]>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the image data from the "ImageData" column
                            byte[] bannerData = (byte[])reader["Banner"];
                            bannerDataList.Add(bannerData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading data from database: " + ex.Message);
                    // Handle exception as needed
                }
            }

            return bannerDataList;
        }
    }
}
