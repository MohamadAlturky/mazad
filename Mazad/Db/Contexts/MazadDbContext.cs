using System.Reflection;
using Mazad.Core.Domain.Regions;
using Mazad.Models;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Core.Shared.Contexts;

public class MazadDbContext : DbContext
{
    public MazadDbContext(DbContextOptions<MazadDbContext> options)
        : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<OfferImage> OfferImages { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<OfferComment> OfferComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
