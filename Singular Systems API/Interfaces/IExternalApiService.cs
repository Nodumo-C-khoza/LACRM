using SingularSystemsAssessment.Entities;

namespace SingularSystemsAssessment.Interfaces
{
    public interface IExternalApiService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Sale>> GetAllSalesAsync();
        Task<IEnumerable<Sale>> GetProductSalesAsync(int productId);
    }
}
