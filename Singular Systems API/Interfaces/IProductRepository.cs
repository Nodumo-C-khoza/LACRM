using SingularSystemsAssessment.Entities;

namespace SingularSystemsAssessment.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddRangeAsync(IEnumerable<Product> products);
        Task<int> CountByProductAsync(int productId);
    }
}
