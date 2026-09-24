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

        private enMode Mode = enMode.AddNew;



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


    }


}
