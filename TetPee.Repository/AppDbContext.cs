using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using TetPee.Repository.Entity;

namespace TetPee.Repository;

public class AppDbContext: DbContext
{
    public static Guid UserId1 = Guid.NewGuid();
    public static Guid UserId2 = Guid.NewGuid();
    public static Guid CategoryParentId1 = Guid.NewGuid();
    public static Guid CategoryParentId2 = Guid.NewGuid();
    public static Guid SellerId = Guid.NewGuid();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User>  Users { get; set; }
    public DbSet<Seller>  Sellers { get; set; }
    public DbSet<Product>  Products { get; set; }
    public DbSet<ProductStorage>  ProductStorages { get; set; }
    public DbSet<Storage>  Storages { get; set; }
    public DbSet<Cart>  Carts { get; set; }
    public DbSet<Inventory>  Inventories { get; set; }
    public DbSet<Order>  Orders { get; set; }
    public DbSet<OrderDetail>  OrderDetails { get; set; }
    public DbSet<ProductCategory>  ProductCategories { get; set; }
    public DbSet<Category>  Categories { get; set; }

    
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ==================== User Configuration ====================
        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.HasIndex(u => u.Email)
                .IsUnique();
            
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            // LastName - required, max 100 characters
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            // ImageUrl - nullable, max 500 characters (URL)
            builder.Property(u => u.ImageUrl)
                .HasMaxLength(500);
            
            // PhoneNumber - nullable, max 20 characters
            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);
            
            // HashedPassword - required, max 500 characters
            builder.Property(u => u.HashedPassword)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");
            
            // Relationship: User has one Seller (one-to-one)
            builder.HasOne(u => u.Seller)
                .WithOne(s => s.User)
                .HasForeignKey<Seller>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // DeleteBehavior.Cascade: Khi một User bị xóa, thì Seller liên quan cũng sẽ bị xóa theo.
            // DeleteBehavior.Restrict: Ngăn chặn việc xóa một User nếu có Seller liên quan tồn tại.
                //(Tham chiếu tới PK tồn tại)
                // 1 Project còn Task thì không xoá được
            // DeleteBehavior.NoAction: Không thực hiện hành động gì đặc biệt khi User bị xóa. ( Gàn giống Restrict, xử lí ở DB)
            // DeleteBehavior.SetNull: Khi một User bị xóa, thì trường UserId trong bảng Seller sẽ được đặt thành NULL.
                //(Áp dụng khi trường FK cho phép NULL)

                List<User> users = new List<User>()
                {
                    new ()
                    {
                        Id = UserId1,
                        Email = "vuduchung@gmail.com",
                        FirstName = "Vuduchung",
                        LastName =  "Vuduchung",
                        HashedPassword = "Vuduchung"
                    },
                    
                    new ()
                    {
                        Id = UserId2,
                        Email = "vuduchungv2@gmail.com",
                        FirstName = "Vuduchung",
                        LastName =  "Vuduchung",
                        HashedPassword = "Vuduchung"
                    }
                };

                for (int i = 0; i < 1000; i++)
                {
                    User newUser = new User()
                    {
                        Id = Guid.NewGuid(),
                        Email = "vuduchung" + i + "@gmail.com",
                        FirstName = "hung" + i,
                        LastName =  "Vu" + i,
                        HashedPassword = "Vuduchung"
                    };
                    users.Add(newUser);
                }
                builder.HasData(users);
        });

        modelBuilder.Entity<Seller>(builder =>
        {
            builder.Property(s => s.TaxCode)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(s => s.CompanyName)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(s => s.CompanyAddress)
                .IsRequired()
                .HasMaxLength(500);

            var seller = new List<Seller>()
            {
                new ()
                {
                    Id = SellerId,
                    TaxCode = "TAXCODE123",
                    CompanyName = "ABC company",
                    CompanyAddress = "123 WP",
                    UserId = UserId1
                }
            };

            builder.HasData(seller);
        });

        modelBuilder.Entity<Category>(builder =>
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);


            var categories = new List<Category>()
            {
                new()
                {
                    Id =  CategoryParentId1,
                    Name = "Áo"
                },
                new()
                {
                    Id =  CategoryParentId2,
                    Name = "Quần"
                },
                new()
                {
                    Id =  Guid.NewGuid(),
                    Name = "Áo giữ nhiệt",
                    ParentId = CategoryParentId1,
                },
                new()
                {
                    Id =  Guid.NewGuid(),
                    Name = "Quần thể thao",
                    ParentId = CategoryParentId2,
                },

            };

            for (int i = 1; i <= 1000; i++)
            {
                var newCategory = new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Áo" + i
                };
                categories.Add(newCategory);
            }
            builder.HasData(categories);

        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            var ProductList = new List<Product>()
            {
                new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Product",
                    Description = "Description",
                    Price = 100,
                    SellerId = SellerId,
                }
            };

            for (int i = 0; i < 1000; i++)
            {
                Product newProduct = new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = "Product" + i,
                    Description = "Description" + 1,
                    Price = i,
                    SellerId = SellerId,
                };
                ProductList.Add(newProduct);
            }
            
            builder.HasData(ProductList);
        });
        
        modelBuilder.Entity<Storage>(builder =>
        {
            

            var storages = new List<Storage>()
            {
                new Storage()
                {
                    Id = Guid.NewGuid(),
                    Price = 100,
                    Type = "Export"
                },
                new Storage()
                {
                    Id = Guid.NewGuid(),
                    Price = 100,
                    Type = "Import"
                }
            };

            for (int i = 0; i < 500; i++)
            {
                Storage newStorageIm = new Storage()
                {
                    Id = Guid.NewGuid(),
                    Price = 100,
                    Type = "Import"
                };
                storages.Add(newStorageIm);
                
                Storage newStorageEx = new Storage()
                {
                    Id = Guid.NewGuid(),
                    Price = 100,
                    Type = "Export"
                };
                storages.Add(newStorageEx);
            }
            
            builder.HasData(storages);
        });
    }
}