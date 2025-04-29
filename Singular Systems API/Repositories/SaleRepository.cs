using Microsoft.EntityFrameworkCore;
using SingularSystemsAssessment.Entities;
using SingularSystemsAssessment.Interfaces;
using System;

namespace SingularSystemsAssessment.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly SingularDbContext _context;

        public SaleRepository(SingularDbContext context)
        { 
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales.ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetFilteredAsync(int? productId, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.Sales.AsQueryable();

            if (productId.HasValue)
                query = query.Where(s => s.ProductId == productId.Value);

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            return await query
                .OrderBy(s => s.SaleDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountFilteredAsync(int? productId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Sales.AsQueryable();

            if (productId.HasValue)
                query = query.Where(s => s.ProductId == productId.Value);

            if (startDate.HasValue)
                query = query.Where(s => s.SaleDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.SaleDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Sale> sales)
        {
            await _context.Sales.AddRangeAsync(sales);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Sale>> GetByProductAsync(
        int productId,
        int page,
        int pageSize)
        {
            return await _context.Sales
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.SaleDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByProductAsync(int productId)
        {
            return await _context.Sales
                .CountAsync(s => s.ProductId == productId);
        }

        public async Task<bool> HasProductSalesAsync(int productId)
        {
            return await _context.Sales
                .AnyAsync(s => s.ProductId == productId);
        }

        public async Task<IEnumerable<Sale>> GetSalesByProductIdAsync(int productId)
        {
            return await _context.Sales
                .Where(s => s.ProductId == productId)
                .ToListAsync();
        }

    }
}
