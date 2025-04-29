using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Models;

namespace SingularSystemsAssessment.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<ProductSalesSummaryViewModel>> GetSalesSummaryAsync();
        Task<IEnumerable<ProductSalesSummaryViewModel>> GetSalesSummaryByProductIdsAsync();
    }
}
