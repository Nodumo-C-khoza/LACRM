using Microsoft.AspNetCore.Mvc;
using SingularSystemsAssessment.Interfaces;

namespace SingularSystemsAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _salesService;

        public SalesController(ISaleService salesService)
        {
            _salesService = salesService;
        }

        // GET api/sales
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int productId,
            [FromQuery] DateTime? saleDate = null)
        {
            try
            {
                var (sales, totalRecords) = await _salesService.GetSalesAsync(
                    productId,
                    saleDate); 

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    Data = sales
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
        }

    }
}
