using Microsoft.AspNetCore.Mvc;
using SingularSystemsAssessment.Interfaces;

namespace SingularSystemsAssessment.Controllers
{

        [ApiController]
        [Route("api/[controller]")]
        public class ProductsController : ControllerBase
        {
            private readonly IProductService _productService;

            public ProductsController(IProductService productService)
            {
                _productService = productService;
            }

            // GET api/products
            [HttpGet]
            public async Task<IActionResult> Get()
            {
                var products = await _productService.GetProductsAsync();
                return Ok(products);
            }

            // GET api/products/sales-summary
            [HttpGet("sales-summary")]
            public async Task<IActionResult> GetSalesSummary()
            {
                var summary = await _productService.GetSalesSummaryByProductIdsAsync();
                return Ok(summary);
            }
        }
    }

