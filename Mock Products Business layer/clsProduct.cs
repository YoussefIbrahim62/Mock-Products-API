using Mock_Products_Data_layer;
using static Mock_Products_Data_layer.clsProductData;

namespace Mock_Products_Business_layer

{
    public class clsProduct
    {

        public static List<ProductDTO> GetAll()
        {
            return clsProductData.GetAllProducts();
        }


    }


}
