using Microsoft.Data.SqlClient;
using System.Data;

namespace Mock_Products_Data_layer
{
    public static class clsProductData
    {
        public class ProductDTO
        { 
            public int Id { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string? Description { get; set; }
            public string Category { get; set; }
            public string? ImageUrl { get; set; }

        }

        private static string _ConnectionString = @"Server=DESKTOP-FGC6F8A\MSSSQLSERVER;Database=ProductsDB;User=sa;Password=123456;TrustServerCertificate=True;";


        public static List<ProductDTO> GetAllProducts()
        {
            var products = new List<ProductDTO>();

            using(SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllProducts", conn))
                {
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            products.Add(
                            new ProductDTO 
                            { 
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Category = reader.GetString(reader.GetOrdinal("Category")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                            });
                        }
                    }


                }

                return products;

            }




        }




    }


}
