using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api_CodeFlow.Model
{
    public partial class database_codeflowContext : DbContext
    {
        public database_codeflowContext()
        {
        }

        public database_codeflowContext(DbContextOptions<database_codeflowContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductGenre> ProductGenres { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductUpdate> ProductUpdates { get; set; } = null!;
        public virtual DbSet<Publisher> Publishers { get; set; } = null!;
        public virtual DbSet<Purchase> Purchases { get; set; } = null!;
        public virtual DbSet<PurchaseList> PurchaseLists { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<StatusType> StatusTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;user=root;password=1941;database=database_codeflow", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Efmigrationshistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__efmigrationshistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });


            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genre");

                entity.Property(e => e.GenreId).HasColumnName("genre_id");

                entity.Property(e => e.GenreName)
                    .HasMaxLength(45)
                    .HasColumnName("genre_name")
                    .HasDefaultValueSql("'default_genre'");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");


                entity.HasIndex(e => e.ProductPublisherId, "product_publisher_id_fk_idx");

                entity.HasIndex(e => e.ProductStatusId, "product_status_fk_idx");

                entity.Property(e => e.ProductId)
                    .ValueGeneratedNever()
                    .HasColumnName("product_id");

                entity.Property(e => e.ProductDescription)
                    .HasMaxLength(600)
                    .HasColumnName("product_description")
                    .HasDefaultValueSql("'default_product_description'");


                entity.Property(e => e.ProductName)
                    .HasMaxLength(70)
                    .HasColumnName("product_name")
                    .HasDefaultValueSql("'default_product_name'");

                entity.Property(e => e.ProductPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("product_price");

                entity.Property(e => e.ProductPublisherId).HasColumnName("product_publisher_id");

                entity.Property(e => e.ProductStatusId)
                    .HasColumnName("product_status_id")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.ProductPublisher)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductPublisherId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_publisher_id_fk");

                entity.HasOne(d => d.ProductStatus)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_status_fk");
            });

            modelBuilder.Entity<ProductGenre>(entity =>
            {
                entity.ToTable("product_genre");

                entity.HasIndex(e => e.GenreId, "genre_fk_idx");

                entity.HasIndex(e => e.ProductId, "product_genre_fk_idx");

                entity.Property(e => e.ProductGenreId).HasColumnName("product_genre_id");

                entity.Property(e => e.GenreId).HasColumnName("genre_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.ProductGenres)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("genre_fk");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductGenres)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_id_fk");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_image");

                entity.HasIndex(e => e.ProductId, "product_image_fk_idx");

                entity.Property(e => e.ProductImageId).HasColumnName("product_image_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductImagePath)
                    .HasMaxLength(100)
                    .HasColumnName("product_image_path");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_image_fk");
            });

            modelBuilder.Entity<ProductUpdate>(entity =>
            {
                entity.ToTable("product_updates");

                entity.HasIndex(e => e.ProductId, "product_updates_product_id_fk_idx");

                entity.Property(e => e.ProductUpdateId).HasColumnName("product_update_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductVersion)
                    .HasMaxLength(45)
                    .HasColumnName("product_version");

                entity.Property(e => e.UpdateStatus).HasColumnName("update_status");

                entity.Property(e => e.UpdateDate).HasColumnName("update_date");

                entity.HasOne(d => d.UpdateStatusNavigation)
                .WithMany(d => d.ProductUpdates)
                   .HasForeignKey(d => d.UpdateStatus)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("update_fk_status");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductUpdates)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_updates_product_id_fk");
            });

            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.ToTable("publisher");

                entity.HasIndex(e => e.PublisherUserId, "publisher_user_id_fk_idx");

                entity.Property(e => e.PublisherId).HasColumnName("publisher_id");

                entity.Property(e => e.PublisherName)
                    .HasMaxLength(100)
                    .HasColumnName("publisher_name");

                entity.Property(e => e.PublisherUserId).HasColumnName("publisher_user_id");

                entity.HasOne(d => d.PublisherUser)
                    .WithMany(p => p.Publishers)
                    .HasForeignKey(d => d.PublisherUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("publisher_user_id_fk");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.PurchasesId)
                    .HasName("PRIMARY");

                entity.ToTable("purchase");

                entity.HasIndex(e => e.PurchaseStatus, "purchase_status_fk_idx");

                entity.HasIndex(e => e.UserId, "purchase_user_id_fk_idx");

                entity.Property(e => e.PurchasesId).HasColumnName("purchases_id");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("datetime")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.PurchaseStatus)
                    .HasColumnName("purchase_status")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.PurchaseStatusNavigation)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.PurchaseStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_status_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Purchases)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_user_id_fk");
            });

            modelBuilder.Entity<PurchaseList>(entity =>
            {
                entity.ToTable("purchase_list");

                entity.HasIndex(e => e.ProductId, "purchase_product_fk_idx");

                entity.HasIndex(e => e.PurchaseId, "purtchase_id_fk_idx");

                entity.Property(e => e.PurchaseListId).HasColumnName("purchase_list_id");

                entity.Property(e => e.Key).HasColumnName("key");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.ProductSpentMoney)
                    .HasPrecision(10, 2)
                    .HasColumnName("product_spent_money");

                entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PurchaseLists)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purchase_product_fk");

                entity.HasOne(d => d.Purchase)
                    .WithMany(p => p.PurchaseLists)
                    .HasForeignKey(d => d.PurchaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("purtchase_id_fk");
            });   

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(45)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.HasIndex(e => e.StatusType, "type_fk_Id_idx");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(45)
                    .HasColumnName("status_name");

                entity.Property(e => e.StatusType).HasColumnName("status_type");

                entity.HasOne(d => d.StatusTypeNavigation)
                    .WithMany(p => p.Statuses)
                    .HasForeignKey(d => d.StatusType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("type_fk_Id");
            });

            modelBuilder.Entity<StatusType>(entity =>
            {
                entity.ToTable("status_type");

                entity.Property(e => e.StatusTypeId).HasColumnName("status_type_id");

                entity.Property(e => e.StatusTypeName)
                    .HasMaxLength(45)
                    .HasColumnName("status_type_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.UserRole, "user_role_fk_idx");

                entity.HasIndex(e => e.UserStatus, "user_status_fk_idx");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(50)
                    .HasColumnName("user_email");

                entity.Property(e => e.UserImage)
                    .HasMaxLength(50)
                    .HasColumnName("user_image");

                entity.Property(e => e.UserLogin)
                    .HasMaxLength(20)
                    .HasColumnName("user_login");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("user_name");

                entity.Property(e => e.UserPassword)
                    .HasMaxLength(20)
                    .HasColumnName("user_password");

                entity.Property(e => e.UserRole)
                    .HasColumnName("user_role")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UserStatus)
                    .HasColumnName("user_status")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.UserRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_fk");

                entity.HasOne(d => d.UserStatusNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_status_fk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
