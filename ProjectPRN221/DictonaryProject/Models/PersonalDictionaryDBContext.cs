using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace DictonaryProject.Models
{
    public partial class PersonalDictionaryDBContext : DbContext
    {
        public PersonalDictionaryDBContext()
        {
        }

        public PersonalDictionaryDBContext(DbContextOptions<PersonalDictionaryDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Dictionary> Dictionaries { get; set; } = null!;
        public virtual DbSet<Meaning> Meanings { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(ConnectionString);
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0858A1A5A")
                    .IsUnique();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(50);
            });

            modelBuilder.Entity<Dictionary>(entity =>
            {
                entity.HasKey(e => e.WordId)
                    .HasName("PK__Dictiona__2C20F046401D3753");

                entity.ToTable("Dictionary");

                entity.Property(e => e.WordId).HasColumnName("WordID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EnglishWord).HasMaxLength(100);

                entity.Property(e => e.IsApproved).HasDefaultValueSql("((0))");

                entity.Property(e => e.Pronunciation).HasMaxLength(100);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Dictionaries)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Dictionar__Creat__30F848ED");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.Words)
                    .UsingEntity<Dictionary<string, object>>(
                        "DictionaryCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("CategoryId").HasConstraintName("FK__Dictionar__Categ__37A5467C"),
                        r => r.HasOne<Dictionary>().WithMany().HasForeignKey("WordId").HasConstraintName("FK__Dictionar__WordI__36B12243"),
                        j =>
                        {
                            j.HasKey("WordId", "CategoryId").HasName("PK__Dictiona__8DB063E467AC43ED");

                            j.ToTable("DictionaryCategories");

                            j.IndexerProperty<int>("WordId").HasColumnName("WordID");

                            j.IndexerProperty<int>("CategoryId").HasColumnName("CategoryID");
                        });
            });

            modelBuilder.Entity<Meaning>(entity =>
            {
                entity.Property(e => e.MeaningId).HasColumnName("MeaningID");

                entity.Property(e => e.EnglishMeaning).HasMaxLength(255);

                entity.Property(e => e.ExampleSentence).HasMaxLength(255);

                entity.Property(e => e.VietnameseMeaning).HasMaxLength(255);

                entity.Property(e => e.WordId).HasColumnName("WordID");

                entity.HasOne(d => d.Word)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(d => d.WordId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Meanings__WordID__3A81B327");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616086F3F40C")
                    .IsUnique();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4F64D03E3")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PasswordHash).HasMaxLength(255);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").HasConstraintName("FK__UserRoles__RoleI__2C3393D0"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").HasConstraintName("FK__UserRoles__UserI__2B3F6F97"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF27604FC8A610AB");

                            j.ToTable("UserRoles");

                            j.IndexerProperty<int>("UserId").HasColumnName("UserID");

                            j.IndexerProperty<int>("RoleId").HasColumnName("RoleID");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
