using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mock_Products_Business_layer;
    
namespace Mock_Products_API.Controllers
{
    //[Route("api/[controller]")]

    [Route("api/Products")]
    [ApiController]
    public class Products : ControllerBase
    {

        [HttpGet(Name ="GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetAllProducts()
        {
            var products = clsProduct.GetAll();

            if(products == null || products.Count == 0 )
                return NotFound("No available products");

            var baseURL = $"{Request.Scheme}://{Request.Host}";

            foreach ( var product in products )
            {
                var imagePath = product.ImageUrl;

                if(!string.IsNullOrEmpty(imagePath))
                    product.ImageUrl = $"{baseURL}{imagePath}";
            }

            return Ok(products);
        }




    }


}
