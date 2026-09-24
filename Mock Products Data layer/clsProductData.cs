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

        public class Create_UpdateProductDTO
        {
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

            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetAllProducts", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                products.Add(new ProductDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Category = reader.GetString(reader.GetOrdinal("Category")),
                                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

                return new List<ProductDTO>();
            }
            catch (Exception ex)
            {
                return new List<ProductDTO>();
            }

            return products;
        }

        public static ProductDTO GetProductById(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetProductById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ProductDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Category = reader.GetString(reader.GetOrdinal("Category")),
                                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        public static List<ProductDTO> GetFilteredProducts(string? query)
        {
            var filteredProducts = new List<ProductDTO>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_SearchProducts", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (query == null)
                            cmd.Parameters.AddWithValue("@Query", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@Query", query);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                filteredProducts.Add(new ProductDTO
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Category = reader.GetString(reader.GetOrdinal("Category")),
                                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new List<ProductDTO>();
            }
            catch (Exception ex)
            {
                return new List<ProductDTO>();
            }

            return filteredProducts;
        }

        public static int AddNewProduct(Create_UpdateProductDTO newProduct)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AddNewProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Title", newProduct.Title);
                        cmd.Parameters.AddWithValue("@Price", newProduct.Price);
                        cmd.Parameters.AddWithValue("@Description", (object?)newProduct.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Category", newProduct.Category);
                        cmd.Parameters.AddWithValue("@ImageUrl", (object?)newProduct.ImageUrl ?? DBNull.Value);

                        var outputProductIdParam = new SqlParameter("@NewProductId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(outputProductIdParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        if (outputProductIdParam.Value != null && outputProductIdParam.Value != DBNull.Value)
                        {
                            return (int)outputProductIdParam.Value;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }

            return -1;
        }

        public static bool UpdateProduct(int id, Create_UpdateProductDTO newProduct)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Title", newProduct.Title);
                        cmd.Parameters.AddWithValue("@Price", newProduct.Price);
                        cmd.Parameters.AddWithValue("@Description", (object?)newProduct.Description ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Category", newProduct.Category);
                        cmd.Parameters.AddWithValue("@ImageUrl", (object?)newProduct.ImageUrl ?? DBNull.Value);

                        conn.Open();

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteProduct(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_DeleteProduct", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", id);

                        conn.Open();

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}