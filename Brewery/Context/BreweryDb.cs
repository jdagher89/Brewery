using Brewery.Models;
using Microsoft.EntityFrameworkCore;

namespace Brewery.Context
{
    public class BreweryDb : DbContext
    {
        public BreweryDb(DbContextOptions<BreweryDb> options):base(options) 
        {

        }
        public DbSet<Beer> Beer { get; set; }
        public DbSet<BrewerSales> BrewerSales { get; set; }
        public DbSet<WholesalerStock> WholesalerStock { get; set; }
    }
}
