using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Mazad.Core.Domain.Categories;

namespace Mazad.Core.Shared.Contexts;

public class MazadDbContext : DbContext
{
    public MazadDbContext(DbContextOptions<MazadDbContext> options)
        : base(options)
    {
    }

    // DbSet for your Category entity
    public DbSet<Category> Categories { get; set; }
    public DbSet<DynamicAttribute> DynamicAttributes { get; set; }
    public DbSet<CategoryAttributes> CategoryDynamicAttributes { get; set; }

    // You can add other DbSets for other entities here

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}