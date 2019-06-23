using Microsoft.EntityFrameworkCore;
using MMS.IdentityManagement.Data.EntityFramework.Entities;

namespace MMS.IdentityManagement.Data.EntityFramework.Repositories
{
    public class ConfigurationContext : DbContext
    {
        public DbSet<ClientEntity> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=MakerIdMgmt;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientEntity>(entity =>
            {
                entity.ToTable("Clients", "dbo");
                entity.HasKey(_ => _.Id).HasName("PK_Clients");

                entity.Property(_ => _.ClientId).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.CreatedWhen).IsRequired();
                entity.Property(_ => _.UpdatedWhen).IsRequired();

                entity.HasIndex(_ => _.ClientId).IsUnique().HasName("U_Clients_ClientId");
            });

            modelBuilder.Entity<ClientSecretEntity>(entity =>
            {
                entity.ToTable("ClientSecrets", "dbo");
                entity.HasKey(_ => _.Id).HasName("PK_ClientSecrets");

                entity.Property(_ => _.SecretId).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.CipherType).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.CipherText).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.CreatedWhen).IsRequired();
                entity.Property(_ => _.UpdatedWhen).IsRequired();

                entity.HasOne(_ => _.Client)
                    .WithMany(_ => _.Secrets)
                    .HasForeignKey(_ => _.ClientId)
                    .IsRequired()
                    .HasConstraintName("FK_ClientSecrets_Clients_ClientId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientSecretTagEntity>(entity =>
            {
                entity.ToTable("ClientSecretTags", "dbo");
                entity.HasKey(_ => _.Id).HasName("PK_ClientSecretTags");

                entity.Property(_ => _.ClientSecretId).IsRequired();
                entity.Property(_ => _.Key).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.Value).IsRequired().IsUnicode();
                entity.Property(_ => _.CreatedWhen).IsRequired();
                entity.Property(_ => _.UpdatedWhen).IsRequired();

                entity.HasOne(_ => _.ClientSecret)
                    .WithMany(_ => _.Tags)
                    .HasForeignKey(_ => _.ClientSecretId)
                    .IsRequired()
                    .HasConstraintName("FK_ClientSecretTags_ClientSecrets_ClientSecretId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ClientTagEntity>(entity =>
            {
                entity.ToTable("ClientTags", "dbo");
                entity.HasKey(_ => _.Id).HasName("PK_ClientTags");

                entity.Property(_ => _.ClientId).IsRequired();
                entity.Property(_ => _.Key).IsRequired().HasMaxLength(255);
                entity.Property(_ => _.Value).IsRequired().IsUnicode();
                entity.Property(_ => _.CreatedWhen).IsRequired();
                entity.Property(_ => _.UpdatedWhen).IsRequired();

                entity.HasOne(_ => _.Client)
                    .WithMany(_ => _.Tags)
                    .HasForeignKey(_ => _.ClientId)
                    .IsRequired()
                    .HasConstraintName("FK_ClientTags_Clients_ClientId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}