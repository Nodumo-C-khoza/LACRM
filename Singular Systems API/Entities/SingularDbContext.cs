using Microsoft.EntityFrameworkCore;

namespace SingularSystemsAssessment.Entities
{
    public class SingularDbContext : DbContext
    {
        public SingularDbContext(DbContextOptions options) : base(options )
        {
            
        }

       public DbSet<Sale> Sales { get; set; }

       public DbSet<Product> Products { get; set; }
    }
}
