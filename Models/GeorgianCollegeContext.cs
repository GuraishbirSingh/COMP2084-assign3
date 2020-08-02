using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment1.Models
{
    public partial class GeorgianCollegeContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public GeorgianCollegeContext()
        {
        }

        public GeorgianCollegeContext(DbContextOptions<GeorgianCollegeContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Carts> Carts { get; set; }
        public virtual DbSet<DonationDetails> DonationDetails { get; set; }
        public virtual DbSet<Donations> Donations { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Students> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //                optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=Georgian College;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Carts>(entity =>
            {
                //entity.Property(e => e.CartId).IsUnicode(false);

                entity.Property(e => e.Credits).HasDefaultValueSql("((1))");
                
                entity.Property(e => e.Username).IsUnicode(false);
                entity.HasOne(d => d.Students)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Carts_StudentId");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.Property(e => e.Coordinator).IsUnicode(false);

                entity.Property(e => e.DelieveryMode).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);
            });

            modelBuilder.Entity<DonationDetails>(entity =>
            {
                entity.Property(e => e.Credits).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Donations)
                    .WithMany(p => p.DonationDetails)
                    .HasForeignKey(d => d.DonationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DonationDetails_DonationId");

                entity.HasOne(d => d.Students)
                    .WithMany(p => p.DonationDetails)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DonationDetails_StudentId");
            });

            modelBuilder.Entity<Donations>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.PostalCode).IsUnicode(false);

                entity.Property(e => e.Province).IsUnicode(false);

                entity.Property(e => e.UserId).IsUnicode(false);
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Nationality).IsUnicode(false);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Students_CourseId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
