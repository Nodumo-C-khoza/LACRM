using SingularSystemsAssessment.Entities;

namespace SingularSystemsAssessment.Interfaces
{
    public interface ISaleService
    {
        Task<(IEnumerable<Sale> Sales, int TotalRecords)> GetSalesAsync(int productId, DateTime? saleDate = null);
        Task InitializeSalesDataAsync(int productId);
    }
}
