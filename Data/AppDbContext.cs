using Microsoft.EntityFrameworkCore;
using messanger.Models;

namespace messanger.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.EncryptedText).IsRequired();
                entity.Property(m => m.CreatedAt).IsRequired();
                entity.HasIndex(m => new { m.SenderId, m.ReceiverId, m.CreatedAt });
            });
        }
    }
}
