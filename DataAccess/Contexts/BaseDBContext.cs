using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DataAccess.Contexts;

public class BaseDBContext : DbContext
{
    public BaseDBContext(DbContextOptions options) : base(options) 
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Word> Words { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Learned> Learneds { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<OTP> OTPs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Word>(w =>
        {
            w.ToTable("Words");
            w.HasKey(w => w.Id);
            w.Property(w => w.Id).HasColumnName("Id");
            w.Property(w => w.CategoryId).HasColumnName("CategoryId");
            w.Property(w => w.Turkish).HasColumnName("Turkish");
            w.Property(w => w.English).HasColumnName("English");
            w.Property(w => w.HasImage).HasColumnName("HasImage");
            w.Property(w => w.Image).HasColumnName("Image");

            w.HasOne(w => w.Category)
                .WithMany(c => c.Words)
                .HasForeignKey(w => w.CategoryId);

            w.HasMany(w => w.Favorites)
                .WithOne(f => f.Word)
                .HasForeignKey(f => f.WordId);

            w.HasMany(w => w.Learneds)
                .WithOne(l => l.Word)
                .HasForeignKey(l => l.WordId);
        });

        modelBuilder.Entity<Category>(c =>
        {
            c.ToTable("Categories");
            c.HasKey(c => c.Id);
            c.Property(c => c.Id).HasColumnName("Id");
            c.Property(c => c.Turkish).HasColumnName("Turkish");
            c.Property(c => c.English).HasColumnName("English");
            c.Property(c => c.HasImage).HasColumnName("HasImage");
            c.Property(c => c.Image).HasColumnName("Image");

            c.HasMany(c => c.Words)
                .WithOne(w => w.Category)
                .HasForeignKey(c => c.CategoryId);
        });

        modelBuilder.Entity<User>(u =>
        {
            u.ToTable("Users");
            u.HasKey(u => u.Id);
            u.Property(u => u.Id).HasColumnName("Id");
            u.Property(u => u.FullName).HasColumnName("FullName");
            u.Property(u => u.Email).HasColumnName("Email");
            u.Property(u => u.PasswordSalt).HasColumnName("PasswordSalt");
            u.Property(u => u.PasswordHash).HasColumnName("PasswordHash");
            u.Property(u => u.IsVerifiedUser).HasColumnName("IsVerifiedUser");
            u.Property(u => u.AutheticatorType).HasColumnName("AutheticatorType");

            u.HasMany(u => u.Favorites)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);

            u.HasMany(u => u.Learneds)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            u.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);
        });
        
        modelBuilder.Entity<Role>(r =>
        {
            r.ToTable("Roles");
            r.HasKey(r => r.Id);
            r.Property(r => r.Id).HasColumnName("Id");
            r.Property(r => r.Name).HasColumnName("Name");

            r.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<UserRoles>(ur =>
        {
            ur.ToTable("UserRoles");
            ur.HasKey(ur => new { ur.UserId, ur.RoleId });
            ur.Property(ur => ur.UserId).HasColumnName("UserId");
            ur.Property(ur => ur.RoleId).HasColumnName("RoleId");

            ur.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            ur.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<Favorite>(f =>
        {
            f.ToTable("Favorites");
            f.HasKey(f => new { f.UserId, f.WordId });
            f.Property(f => f.UserId).HasColumnName("UserId");
            f.Property(f => f.WordId).HasColumnName("WordId");

            f.HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            f.HasOne(f => f.Word)
                .WithMany(w => w.Favorites)
                .HasForeignKey(f => f.WordId);
        });

        modelBuilder.Entity<Learned>(l =>
        {
            l.ToTable("Learneds");
            l.HasKey(l => new { l.UserId, l.WordId });
            l.Property(l => l.UserId).HasColumnName("UserId");
            l.Property(l => l.WordId).HasColumnName("WordId");

            l.HasOne(l => l.User)
                .WithMany(u => u.Learneds)
                .HasForeignKey(l => l.UserId);

            l.HasOne(l => l.Word)
                .WithMany(w => w.Learneds)
                .HasForeignKey(l => l.WordId);
        });


        modelBuilder.Entity<OTP>(o =>
        {
            o.ToTable("Otp");
            o.HasKey(o => o.UserId);
            o.Property(o => o.UserId).HasColumnName("UserId");
            o.Property(o => o.ExpiryTime).HasColumnName("ExpiryTime");
            o.Property(o => o.Code).HasColumnName("Code");
        });
    }
}