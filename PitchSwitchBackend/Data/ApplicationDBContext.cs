using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferRumour> TransferRumours { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "fca020df-3835-438d-ab04-49bcae94904a",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2e818aa9-3ac2-4e90-bde8-6e4f0c29455b",
                    Name = "Journalist",
                    NormalizedName = "JOURNALIST"
                },
                new IdentityRole
                {
                    Id = "e01d1c56-5fd5-41e9-be34-4a9e9f351ae7",
                    Name = "User",
                    NormalizedName = "USER"
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);

            SetForeignKeys(builder);

        }

        private void SetForeignKeys(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .HasOne(u => u.FavouriteClub)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.FavouriteClubId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Player>()
                .HasOne(p => p.Club)
                .WithMany(c => c.Players)
                .HasForeignKey(u => u.ClubId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Transfer>()
                .HasOne(t => t.Player)
                .WithMany()
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Transfer>()
                .HasOne(t => t.SellingClub)
                .WithMany()
                .HasForeignKey(t => t.SellingClubId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transfer>()
                .HasOne(t => t.BuyingClub)
                .WithMany()
                .HasForeignKey(t => t.BuyingClubId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransferRumour>()
                .HasOne(tr => tr.Player)
                .WithMany()
                .HasForeignKey(tr => tr.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TransferRumour>()
                .HasOne(tr => tr.SellingClub)
                .WithMany()
                .HasForeignKey(tr => tr.SellingClubId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransferRumour>()
                .HasOne(tr => tr.BuyingClub)
                .WithMany()
                .HasForeignKey(tr => tr.BuyingClubId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
