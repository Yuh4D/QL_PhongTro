using Microsoft.EntityFrameworkCore;
using WebQLPT.Models;

namespace WebQLPT.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PhongTro> PhongTros { get; set; }
        public DbSet<KhachThue> KhachThues { get; set; }
        public DbSet<ChuTro> ChuTros { get; set; }
        public DbSet<DangTin> DangTins { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<HopDong> HopDongs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDonChiTiet> HoaDonChiTiets { get; set; } 
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<KhachThue>()
                .HasOne(k => k.PhongTro)
                .WithMany(p => p.KhachThues)
                .HasForeignKey(k => k.PhongTroId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DangTin>()
                .HasOne(d => d.PhongTro)
                .WithMany()
                .HasForeignKey(d => d.PhongTroId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DangTin>()
                .HasOne(d => d.ChuTro)
                .WithMany()
                .HasForeignKey(d => d.ChuTroId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.PhongTro)
                .WithMany()
                .HasForeignKey(h => h.PhongTroId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<HopDong>()
                .HasOne(h => h.KhachThue)
                .WithMany()
                .HasForeignKey(h => h.KhachThueId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.PhongTro)
                .WithMany()
                .HasForeignKey(h => h.PhongTroId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.KhachThue)
                .WithMany()
                .HasForeignKey(h => h.KhachThueId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }


}
