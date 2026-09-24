using Mock_Products_Data_layer;
using static Mock_Products_Data_layer.clsProductData;

namespace Mock_Products_Business_layer

{
    public class clsProduct
    {
        public enum enMode
        {
            AddNew = 0,
            Update = 1
        }
        public int Id { get; private set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public string? ImageUrl { get; set; }
        public ProductDTO ProductDTO 
        {
            get
            {
                return new ProductDTO() 
                {
                    Id = this.Id, 
                    Title = this.Title,
                    Price = this.Price,
                    Description = this.Description,
                    Category = this.Category,
                    ImageUrl = this.ImageUrl
                }; 
            }
        }
        private Create_UpdateProductDTO AddNew_UpdateProductDto
        {
            get
            {
                return new Create_UpdateProductDTO()
                {
                    Title = this.Title,
                    Price = this.Price,
                    Description = this.Description,
                    Category = this.Category,
                    ImageUrl = this.ImageUrl
                };
            }
        }

        private enMode Mode;


        public clsProduct(Create_UpdateProductDTO newProductDto)
        {
            this.Title = newProductDto.Title;
            this.Price = newProductDto.Price;
            this.Description = newProductDto.Description;
            this.Category = newProductDto.Category;
            this.ImageUrl = newProductDto.ImageUrl;
            this.Mode = enMode.AddNew;
        }

        private clsProduct(ProductDTO productDto, enMode mode)
        {
            this.Id = productDto.Id;
            this.Title = productDto.Title;
            this.Price = productDto.Price;
            this.Description = productDto.Description;
            this.Category = productDto.Category;
            this.ImageUrl = productDto.ImageUrl;
            this.Mode = mode;
        }

        public static clsProduct FindById(int id)
        {
            var productDTO = clsProductData.GetProductById(id);

            if(productDTO != null)
            {
                return new clsProduct(productDTO, enMode.Update);  
            }

            return null;
        }

        public static List<ProductDTO> GetAll()
        {
            return clsProductData.GetAllProducts();
        }

        public static List<ProductDTO> GetFilteredProducts(string? query)
        {
            return clsProductData.GetFilteredProducts(query);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    {
                        if(_AddNewProduct())
                        {
                            this.Mode = enMode.Update;
                            return true;
                        }
                        return false;
                    }
                case enMode.Update:
                    {
                        return _UpdateProduct();
                    }
                default:
                    return false;   
            }

        }


        public bool Delete()
        {
            return clsProductData.DeleteProduct(this.Id);
        }

        public static bool DeleteProduct(int id)
        {
            return clsProductData.DeleteProduct(id);
        }



        private bool _AddNewProduct()
        {
            int newProductId = clsProductData.AddNewProduct(this.AddNew_UpdateProductDto);
            this.Id = newProductId;

            return newProductId != -1;
        }

        private bool _UpdateProduct()
        {
            return clsProductData.UpdateProduct(this.Id, AddNew_UpdateProductDto);
        }




    }


}
