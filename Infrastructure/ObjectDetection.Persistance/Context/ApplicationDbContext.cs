using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ObjectDetection.Domain.Entities;

namespace ObjectDetection.Persistance.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
        public DbSet<BoundingBox> BoundingBoxes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Image>()
                .HasMany(i => i.BoundingBoxes)
                .WithOne(b => b.Image)
                .HasForeignKey(b => b.ImageId);
        }
    }
}
