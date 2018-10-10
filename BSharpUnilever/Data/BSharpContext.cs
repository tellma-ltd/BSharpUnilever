using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BSharpUnilever.Data
{
    /// <summary>
    /// One context for both the application and the identity, 
    /// we do not have to separate them for such a small app
    /// </summary>
    public class BSharpContext : IdentityUserContext<User>
    {
        // This is necessary so the context has a chance to receive 
        // the options from the DI (e.g. the connection string)
        public BSharpContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<SupportRequest> SupportRequests { get; set; }

        public DbSet<SupportRequestLineItem> SupportRequestLineItems { get; set; }

        public DbSet<StateChange> StateChanges { get; set; }

        public DbSet<GeneratedDocument> GeneratedDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // EF Core by default sets cascade delete to true, (or ClientSetNull for nullable foreign keys)
            // Here we override this behavior using the EF Core fluent API for data modeling
            builder.Entity<SupportRequest>().
                HasOne(e => e.Store)
                .WithMany(e => e.SupportRequests).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupportRequest>().
                HasOne(e => e.Manager)
                .WithMany(e => e.ManagedRequests).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupportRequest>().
                HasOne(e => e.AccountExecutive)
                .WithMany(e => e.Requests).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupportRequestLineItem>().
                HasOne(e => e.Product)
                .WithMany(e => e.SupportRequestLineItems).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Store>().
                HasOne(e => e.AccountExecutive)
                .WithMany(e => e.AssignedStores).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
