using Microsoft.EntityFrameworkCore;

namespace IntegrationSample
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }
        DbSet<Customer> customer { get; set; }
    }
}
