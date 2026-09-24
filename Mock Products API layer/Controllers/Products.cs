using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mock_Products_Business_layer;
using Mock_Products_Data_layer;
using static Mock_Products_API.Controllers.Products;
using static Mock_Products_Data_layer.clsProductData;

namespace Mock_Products_API.Controllers
{
    //[Route("api/[controller]")]

    [Route("api/Products")]
    [ApiController]
    public class Products : ControllerBase
    {

        [HttpGet(Name = "GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsProductData.ProductDTO>> GetAllProducts()
        {
            var products = clsProduct.GetAll();

            if (products == null || products.Count == 0)
                return NotFound("No available products");

            var baseURL = $"{Request.Scheme}://{Request.Host}";

            foreach (var product in products)
            {
                var imagePath = product.ImageUrl;

                if (!string.IsNullOrEmpty(imagePath))
                    product.ImageUrl = $"{baseURL}{imagePath}";
            }

            return Ok(products);
        }



        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsProductData.ProductDTO> GetProductById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            clsProduct FoundProduct = clsProduct.FindById(id);

            if (FoundProduct == null)
                return NotFound($"Product with id= {id} doesn't exist");



            var productDTO = FoundProduct.ProductDTO;

            if (!string.IsNullOrEmpty(FoundProduct.ImageUrl))
            {
                var baseURL = $"{Request.Scheme}://{Request.Host}" + FoundProduct.ImageUrl;
                productDTO.ImageUrl = baseURL;
            }


            return Ok(productDTO);

        }



        [HttpGet("Search", Name = "GetFilteredProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<clsProductData.ProductDTO>> GetFilteredProducts(string? query)
        {
            var FilteredProducts = clsProduct.GetFilteredProducts(query);

            if (FilteredProducts == null || FilteredProducts.Count == 0)
                return NotFound("No available products");

            var baseURL = $"{Request.Scheme}://{Request.Host}";

            foreach (var product in FilteredProducts)
            {
                var imagePath = product.ImageUrl;

                if (!string.IsNullOrEmpty(imagePath))
                    product.ImageUrl = $"{baseURL}{imagePath}";
            }

            return Ok(FilteredProducts);

        }



        private bool InValidateProduct(PUT_POSTProductDTOAPI newProduct)
        {
            if (newProduct == null)
                return true;

            if (string.IsNullOrEmpty(newProduct.Title)
               || string.IsNullOrEmpty(newProduct.Category))
                return true;

            if (newProduct.Price <= 0)
                return true;

            return false;
        }


        public class PUT_POSTProductDTOAPI
        {
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string? Description { get; set; }
            public string Category { get; set; }
            public IFormFile? ImageFile { get; set; }
        }

        public class PATCHProductDTOAPI
        {
            public string? Title { get; set; }
            public decimal? Price { get; set; }
            public string? Description { get; set; }
            public string Category { get; set; }
            public IFormFile? ImageFile { get; set; }
        }





        private clsProductData.Create_UpdateProductDTO ToCreatedProductDTO(PUT_POSTProductDTOAPI newProductAPI)
        {
            var newProductDataLayer = new clsProductData.Create_UpdateProductDTO();

            newProductDataLayer.Title = newProductAPI.Title;
            newProductDataLayer.Price = newProductAPI.Price;
            newProductDataLayer.Description = newProductAPI.Description;
            newProductDataLayer.Category = newProductAPI.Category;


            return newProductDataLayer;
        }

        [HttpPost(Name = "CreateNewProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<clsProductData.ProductDTO> CreateNewProduct([FromForm] PUT_POSTProductDTOAPI newProductApi)
        {
            if (InValidateProduct(newProductApi))
                return BadRequest("Invalid information");


            var newProductDataLayer = new clsProductData.Create_UpdateProductDTO();


            string? imageRelativePath = null;

            if (newProductApi.ImageFile != null && newProductApi.ImageFile.Length > 0)
            {

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(newProductApi.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Invalid image format. Allowed formats: .jpg, .jpeg, .png");


                string fileName = $"{Guid.NewGuid()}{extension}";


                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fullPath = Path.Combine(uploadsFolder, fileName);


                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    newProductApi.ImageFile.CopyTo(stream);
                }


                imageRelativePath = $"/images/Products/{fileName}";
            }

            newProductDataLayer = ToCreatedProductDTO(newProductApi);
            newProductDataLayer.ImageUrl = imageRelativePath;


            clsProduct NewProduct = new clsProduct(newProductDataLayer);

            if (NewProduct.Save())
            {
                var responseDto = NewProduct.ProductDTO;

                if (!string.IsNullOrEmpty(responseDto.ImageUrl))
                {
                    responseDto.ImageUrl = $"{Request.Scheme}://{Request.Host}{responseDto.ImageUrl}";
                }

                return CreatedAtRoute("GetProductById", new { id = NewProduct.Id }, responseDto);
            }
            return BadRequest("Couldn't save the product");

        }




        [HttpPut("{id}", Name = "UpdateProductPut")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ProductDTO> UpdateProductPut(int id, [FromForm] PUT_POSTProductDTOAPI updatedProductApi)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            
            if (InValidateProduct(updatedProductApi))
            {
                return BadRequest("Title, Category, and a valid Price are required for full update.");
            }

            clsProduct product = clsProduct.FindById(id);

            if (product == null)
                return NotFound($"Product with id = {id} doesn't exist");

            
            product.Title = updatedProductApi.Title;
            product.Category = updatedProductApi.Category;
            product.Price = updatedProductApi.Price;
            product.Description = updatedProductApi.Description;

            
            if (updatedProductApi.ImageFile != null && updatedProductApi.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(updatedProductApi.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Invalid image format. Allowed formats: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                string fileName = $"{Guid.NewGuid()}{extension}";
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    updatedProductApi.ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/images/Products/{fileName}";
            }

            if (product.Save())
            {
                var updatedDto = product.ProductDTO;

                if (!string.IsNullOrEmpty(updatedDto.ImageUrl))
                {
                    updatedDto.ImageUrl = $"{Request.Scheme}://{Request.Host}{updatedDto.ImageUrl}";
                }

                return Ok(updatedDto);
            }

            return BadRequest("Couldn't update the product");
        }




        [HttpPatch("{id}", Name = "PatchProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ProductDTO> PatchProduct(int id, [FromForm] PATCHProductDTOAPI patchProductApi)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            clsProduct product = clsProduct.FindById(id);

            if (product == null)
                return NotFound($"Product with id = {id} doesn't exist");

            
            if (!string.IsNullOrWhiteSpace(patchProductApi.Title))
                product.Title = patchProductApi.Title;

            if (!string.IsNullOrWhiteSpace(patchProductApi.Category))
                product.Category = patchProductApi.Category;

            if (patchProductApi.Price.HasValue && patchProductApi.Price.Value > 0)
                product.Price = patchProductApi.Price.Value;

            if (patchProductApi.Description != null)
                product.Description = patchProductApi.Description;

            
            if (patchProductApi.ImageFile != null && patchProductApi.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(patchProductApi.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Invalid image format. Allowed formats: .jpg, .jpeg, .png");

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                string fileName = $"{Guid.NewGuid()}{extension}";
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Products");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    patchProductApi.ImageFile.CopyTo(stream);
                }

                product.ImageUrl = $"/images/Products/{fileName}";
            }

            if (product.Save())
            {
                var updatedDto = product.ProductDTO;

                if (!string.IsNullOrEmpty(updatedDto.ImageUrl))
                {
                    updatedDto.ImageUrl = $"{Request.Scheme}://{Request.Host}{updatedDto.ImageUrl}";
                }

                return Ok(updatedDto);
            }

            return BadRequest("Couldn't patch the product");
        }



        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            clsProduct deletedProduct = clsProduct.FindById(id);

            if (deletedProduct == null)
                return NotFound($"Product with id = {id} doesn't exist");

            
            if (!string.IsNullOrEmpty(deletedProduct.ImageUrl))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", deletedProduct.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            
            if (deletedProduct.Delete())
                return Ok($"Product with id = {id} has been deleted successfully");

            return BadRequest("Couldn't delete the product");
        }




    }


}
