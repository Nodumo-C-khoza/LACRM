using Microsoft.EntityFrameworkCore;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;

namespace SingularSystemsAssessment.Repositories
{
    public class ProductRepository : IProductRepository
    {

            private readonly SingularDbContext _context;
            public ProductRepository(SingularDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Product>> GetAllAsync()
            {
                return await _context.Products.ToListAsync();
            }

            public async Task<Product?> GetByIdAsync(int id)
            {
                return await _context.Products.FindAsync(id);
            }

            public async Task AddRangeAsync(IEnumerable<Product> products)
            {
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }

            public async Task<int> CountByProductAsync(int productId)
            {
                var x = await _context.Sales.CountAsync
                        (s => s.ProductId == productId);
                return x;
            }
    }
}
