using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;


namespace Level3
{
    // enum definition of status marking
    public enum AssetLifeStatus
    {
        Normal,
        Yellow,
        Red,
        Grey
    }

    // enum definition of countries
    public enum Country
    {
        UnitedStates,
        Germany,
        Sweden
    }

    // Base Asset Abstract Class 

    public abstract class Asset
    {
        [Key] // denotation of primary key
        public int Id { get; set; }

        [Required] // denotation if required field
        public required string Manufacturer { get; set; }

        [Required]
        public required string Model { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public DateTime EndOfLifeDate => PurchaseDate.AddYears(3); // after 3 years the asset is obsolete 

        public bool IsExpired => DateTime.Now > EndOfLifeDate;

        //  method to check proximity to end of life
        public bool IsNearEndOfLife
        {
            get
            {
                var now = DateTime.Now;
                var endOfLifeDate = EndOfLifeDate;
                var timeToEndOfLife = endOfLifeDate - now;

                // RED if less than 3 months away from 3 years
                if (timeToEndOfLife.TotalDays <= 90)  // 3 months = 90 days
                {
                    return true;
                }

                return false;
            }
        }

        // property to determine color coding
        public AssetLifeStatus LifeStatus
        {
            get
            {
                var now = DateTime.Now;
                var endOfLifeDate = EndOfLifeDate;
                var timeToEndOfLife = endOfLifeDate - now;

                // Grey if 3 years have passed i.e. obsolete
                if (now > endOfLifeDate)
                {
                    return AssetLifeStatus.Grey;
                }
                // RED if less than 3 months away from 3 years
                else if (timeToEndOfLife.TotalDays <= 90)  // 3 months = 90 days
                {
                    return AssetLifeStatus.Red;
                }
                // YELLOW if less than 6 months away from 3 years
                else if (timeToEndOfLife.TotalDays <= 180)  // 6 months = 180 days
                {
                    return AssetLifeStatus.Yellow;
                }

                return AssetLifeStatus.Normal;
            }
        }

        // some properties for Office class/model
        public int? OfficeId { get; set; }
        public Office? Office { get; set; }

        public ICollection<AssetUser> AssetUsers { get; set; } // Navigation property from generic class ICollection
    }


    // Laptop Subclasses derived from abstract superclass Asset
    public class MacBook : Asset
    {
        public required string ProcessorType { get; set; }
        public int MemoryGB { get; set; }
    }

    public class AsusLaptop : Asset
    {
        public required string ProcessorType { get; set; }
        public int MemoryGB { get; set; }
    }

    public class LenovoLaptop : Asset
    {
        public required string ProcessorType { get; set; }
        public int MemoryGB { get; set; }
    }

    // Mobile Phone Subclasses derived  from abstract superclass Asset
    public class Iphone : Asset
    {
        public required string StorageCapacity { get; set; }
        public required string Color { get; set; }
    }

    public class SamsungPhone : Asset
    {
        public required string StorageCapacity { get; set; }
        public required string Color { get; set; }
    }

    public class NokiaPhone : Asset
    {
        public required string StorageCapacity { get; set; }
        public required string Color { get; set; }
    }

    // enum and class for Office
    public class Office
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public Country Country { get; set; }

        // Navigation property for assets
        public List<Asset> Assets { get; set; } = new List<Asset>();
    }

    // Enhanced Database Context
    public class AssetTrackingContext : DbContext // derived from superclass DbContext
    {
        public DbSet<MacBook> MacBooks { get; set; }
        public DbSet<AsusLaptop> AsusLaptops { get; set; }
        public DbSet<LenovoLaptop> LenovoLaptops { get; set; }
        public DbSet<Iphone> Iphones { get; set; }
        public DbSet<SamsungPhone> SamsungPhones { get; set; }
        public DbSet<NokiaPhone> NokiaPhones { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<AssetUser> AssetUsers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Explicitly check if options are not already configured
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=AssetTracking.db"); // create a new db
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssetUser>()
                .HasKey(au => new { au.AssetId, au.UserId }); // Composite key for many to many relation Asset <-> User

            modelBuilder.Entity<AssetUser>()
                .HasOne(au => au.Asset)
                .WithMany(a => a.AssetUsers)
                .HasForeignKey(au => au.AssetId);

            modelBuilder.Entity<AssetUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.AssetUsers)
                .HasForeignKey(au => au.UserId);

            // Configure relationship between Asset and Office
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Office)
                .WithMany(o => o.Assets)
                .HasForeignKey(a => a.OfficeId)
                .IsRequired(false);  // Make the relationship optional

            // Seed Office data first
            modelBuilder.Entity<Office>().HasData(
                new Office
                {
                    Id = 1,
                    Name = "San Francisco HQ",
                    Country = Country.UnitedStates
                },
                new Office
                {
                    Id = 2,
                    Name = "Berlin Office",
                    Country = Country.Germany
                },
                new Office
                {
                    Id = 3,
                    Name = "Stockholm Office",
                    Country = Country.Sweden
                }
            );

            // MacBook Seeding - Ensure OfficeId is set
            modelBuilder.Entity<MacBook>().HasData(
                new MacBook
                {
                    Id = 1,
                    Manufacturer = "Apple",
                    Model = "MacBook Pro 16\"",
                    PurchaseDate = new DateTime(2023, 1, 15),
                    Price = 2499.99m,
                    ProcessorType = "M1 Pro",
                    MemoryGB = 16,
                    OfficeId = 1  // Explicitly set San Francisco HQ
                },
                new MacBook
                {
                    Id = 2,
                    Manufacturer = "Apple",
                    Model = "MacBook Air",
                    PurchaseDate = new DateTime(2023, 6, 1),
                    Price = 1299.99m,
                    ProcessorType = "M1",
                    MemoryGB = 8,
                    OfficeId = 2  // Explicitly set Berlin Office
                }
            );

            // Asus Laptop Seeding
            modelBuilder.Entity<AsusLaptop>().HasData(
                new AsusLaptop
                {
                    Id = 3,
                    Manufacturer = "Asus",
                    Model = "Zephyrus G14",
                    PurchaseDate = new DateTime(2023, 3, 20),
                    Price = 1799.99m,
                    ProcessorType = "AMD Ryzen 9",
                    MemoryGB = 32,
                    OfficeId = 3  // Explicitly set Stockholm Office
                },
                new AsusLaptop
                {
                    Id = 4,
                    Manufacturer = "Asus",
                    Model = "ZenBook 14",
                    PurchaseDate = new DateTime(2021, 6, 10),
                    Price = 1099.99m,
                    ProcessorType = "Intel Core i7",
                    MemoryGB = 16,
                    OfficeId = 1  // Explicitly set San Francisco HQ
                }
            );

            // Lenovo Laptop Seeding
            modelBuilder.Entity<LenovoLaptop>().HasData(
                new LenovoLaptop
                {
                    Id = 5,
                    Manufacturer = "Lenovo",
                    Model = "ThinkPad X1",
                    PurchaseDate = new DateTime(2022, 1, 25),
                    Price = 1899.99m,
                    ProcessorType = "Intel Core i7",
                    MemoryGB = 16,
                    OfficeId = 2  // Berlin Office
                },
                new LenovoLaptop
                {
                    Id = 6,
                    Manufacturer = "Lenovo",
                    Model = "Yoga 9i",
                    PurchaseDate = new DateTime(2022, 5, 5),
                    Price = 1499.99m,
                    ProcessorType = "Intel Core i5",
                    MemoryGB = 12,
                    OfficeId = 3  // Stockholm Office
                }
            );

            // iPhone Seeding
            modelBuilder.Entity<Iphone>().HasData(
                new Iphone
                {
                    Id = 7,
                    Manufacturer = "Apple",
                    Model = "iPhone 13 Pro",
                    PurchaseDate = new DateTime(2021, 8, 1),
                    Price = 1099.99m,
                    StorageCapacity = "256GB",
                    Color = "Graphite",
                    OfficeId = 1  // San Francisco HQ
                },
                new Iphone
                {
                    Id = 8,
                    Manufacturer = "Apple",
                    Model = "iPhone 12",
                    PurchaseDate = new DateTime(2021, 11, 15),
                    Price = 799.99m,
                    StorageCapacity = "128GB",
                    Color = "Blue",
                    OfficeId = 3  // Stockholm Office
                }
            );

            // Samsung Phone Seeding
            modelBuilder.Entity<SamsungPhone>().HasData(
                new SamsungPhone
                {
                    Id = 9,
                    Manufacturer = "Samsung",
                    Model = "Galaxy S21",
                    PurchaseDate = new DateTime(2021, 1, 10),
                    Price = 1199.99m,
                    StorageCapacity = "512GB",
                    Color = "Phantom Black",
                    OfficeId = 2  // Berlin Office
                },
                new SamsungPhone
                {
                    Id = 10,
                    Manufacturer = "Samsung",
                    Model = "Galaxy A52",
                    PurchaseDate = new DateTime(2022, 3, 20),
                    Price = 499.99m,
                    StorageCapacity = "128GB",
                    Color = "Awesome Blue",
                    OfficeId = 3  // Stockholm Office
                }
            );

            // Nokia Phone Seeding
            modelBuilder.Entity<NokiaPhone>().HasData(
                new NokiaPhone
                {
                    Id = 11,
                    Manufacturer = "Nokia",
                    Model = "8.3 5G",
                    PurchaseDate = new DateTime(2022, 2, 14),
                    Price = 599.99m,
                    StorageCapacity = "128GB",
                    Color = "Polar Night",
                    OfficeId = 1  // San Francisco HQ
                },
                new NokiaPhone
                {
                    Id = 12,
                    Manufacturer = "Nokia",
                    Model = "G20",
                    PurchaseDate = new DateTime(2022, 2, 5),
                    Price = 249.99m,
                    StorageCapacity = "64GB",
                    Color = "Night",
                    OfficeId = 2  // Berlin Office
                }
            );

            // User seeding users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Alice" },
                new User { Id = 2, Name = "Robert" },
                new User { Id = 3, Name = "Carl" },
                new User { Id = 4, Name = "Diana" },
                new User { Id = 5, Name = "Eric" }
            );

            // AssetUser seeding to ensure all assets are assigned to users
            modelBuilder.Entity<AssetUser>().HasData(
                new AssetUser { AssetId = 1, UserId = 1 }, // Alice uses Asset 1
                new AssetUser { AssetId = 2, UserId = 1 }, // Alice uses Asset 2
                new AssetUser { AssetId = 3, UserId = 2 }, // Robert uses Asset 3
                new AssetUser { AssetId = 4, UserId = 2 }, // Robert uses Asset 4
                new AssetUser { AssetId = 5, UserId = 3 }, // Carl uses Asset 5
                new AssetUser { AssetId = 6, UserId = 3 }, // Carl uses Asset 6
                new AssetUser { AssetId = 1, UserId = 4 }, // Diana uses Asset 1
                new AssetUser { AssetId = 2, UserId = 5 }, // Eric uses Asset 2
                new AssetUser { AssetId = 3, UserId = 4 }, // Diana uses Asset 3
                new AssetUser { AssetId = 4, UserId = 5 }, // Eric uses Asset 4
                new AssetUser { AssetId = 5, UserId = 1 }, // Alice uses Asset 5
                new AssetUser { AssetId = 6, UserId = 2 }  // Robert uses Asset 6
            );

            base.OnModelCreating(modelBuilder);
        }


    }


    // Enhanced Asset Management Service with Full CRUD
    public class AssetManagementService
    {
        private readonly AssetTrackingContext _context;

        public AssetManagementService()
        {
            _context = new AssetTrackingContext();
            _context.Database.EnsureCreated();
        }

        // Create asset
        public void AddAsset<T>(T asset) where T : Asset
        {
            _context.Set<T>().Add(asset);
            _context.SaveChanges();
            Console.WriteLine($"{typeof(T).Name} asset added successfully!");
        }

        // Read single Asset
        public T GetAssetById<T>(int id) where T : Asset
        {
            return _context.Set<T>().Find(id);
        }

        // Read all Assets with sorting
        public List<Asset> GetAllAssetsSorted()
        {
            // Get all assets across all types
            var laptops = new List<Asset>()
                .Concat(_context.MacBooks.Cast<Asset>())
                .Concat(_context.AsusLaptops.Cast<Asset>())
                .Concat(_context.LenovoLaptops.Cast<Asset>());

            var phones = new List<Asset>()
                .Concat(_context.Iphones.Cast<Asset>())
                .Concat(_context.SamsungPhones.Cast<Asset>())
                .Concat(_context.NokiaPhones.Cast<Asset>());

            // Combine and sort
            return laptops.Concat(phones)
                .OrderBy(a => a.GetType().Name.EndsWith("hone")) // Computers first then phones
                .ThenBy(a => a.PurchaseDate)
                .ToList();
        }

        // Update asset price
        public void UpdateAsset<T>(T asset) where T : Asset
        {
            _context.Set<T>().Update(asset);
            _context.SaveChanges();
            Console.WriteLine($"{typeof(T).Name} asset price updated successfully!");
        }

        // Delete asset
        public void DeleteAsset<T>(int id) where T : Asset
        {
            var asset = _context.Set<T>().Find(id);
            if (asset != null)
            {
                _context.Set<T>().Remove(asset);
                _context.SaveChanges();
                Console.WriteLine($"{typeof(T).Name} asset deleted successfully!");
            }
            else
            {
                Console.WriteLine("Asset not found.");
            }
        }

        // Display all assets with color coding for status
        public void DisplayAllAssets()
        {
            var sortedAssets = GetAllAssetsSorted();

            // Print header
            Console.WriteLine("\n{0,-3} {1,-15} {2,-10} {3,-15} {4,-12} {5, 10} {6,15} {7,10} {8,5}",
                "ID", "Type", "Brand", "Model", "Purchase", "$ Price", "End of Life", "Status", "Office");
            Console.WriteLine(new string('_', 110));

            foreach (var asset in sortedAssets)
            {
                // Set color based on life status
                switch (asset.LifeStatus)
                {
                    case AssetLifeStatus.Red:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case AssetLifeStatus.Yellow:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case AssetLifeStatus.Grey:
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    default:
                        Console.ResetColor();
                        break;
                }

                // Get office name, use "Unassigned" if no office
                string officeName = asset.OfficeId.ToString() ?? "Unassigned";

                // Display asset details in a formatted row
                Console.WriteLine("{0,3} {1,-15} {2,-10} {3,-15} {4,-12} {5, 10} {6,15:d} {7,10} {8,5}",
                    asset.Id,
                    asset.GetType().Name,
                    asset.Manufacturer,
                    asset.Model,
                    asset.PurchaseDate.ToString("d"),
                    asset.Price,
                    asset.EndOfLifeDate,
                    asset.LifeStatus == AssetLifeStatus.Grey ? "Expired" :
                        (asset.LifeStatus == AssetLifeStatus.Red ? "Near End" :
                        (asset.LifeStatus == AssetLifeStatus.Yellow ? "Expiring" : "Active")),
                    officeName);

                // Reset color after each asset
                Console.ResetColor();
            }

            // Print total count
            Console.WriteLine("\nTotal Assets: {0}", sortedAssets.Count);
        }


        // Method to assign an asset to an office
        public void AssignAssetToOffice<T>(int assetId, int officeId) where T : Asset
        {
            var asset = _context.Set<T>().Find(assetId);
            var office = _context.Offices.Find(officeId);

            if (asset != null && office != null)
            {
                asset.OfficeId = officeId;
                _context.SaveChanges();
                Console.WriteLine($"Asset {assetId} assigned to {office.Name}");
            }
            else
            {
                Console.WriteLine("Asset or Office not found.");
            }
        }

        // Method to get assets by office
        public List<Asset> GetAssetsByOffice(int officeId)
        {
            return _context.Set<Asset>()
                .Where(a => a.OfficeId == officeId)
                .ToList();
        }

        // Method AssetManagementService get assets by user
        public List<Asset> GetAssetsByUser(int userId)
        {
            try
            {
                return _context.AssetUsers
                    .Where(au => au.UserId == userId)
                    .Select(au => au.Asset)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving assets for user {userId}: {ex.Message}");
                return new List<Asset>(); // Return empty list instead of null
            }
        }
    }

    // Currency Conversion Utility
    public static class CurrencyConverter
    {
        // These rates are updated 2024-12-08
        private static readonly Dictionary<Country, decimal> ExchangeRates = new Dictionary<Country, decimal>
        {
            { Country.UnitedStates, 1.0m },      // Base currency (USD)
            { Country.Germany, 0.95m },          // Euro rate
            { Country.Sweden, 10.94m }           // Swedish Krona rate
        };

        private static readonly Dictionary<Country, string> CurrencySymbols = new Dictionary<Country, string>
        {
            { Country.UnitedStates, "$ " },
            { Country.Germany, "€ " },
            { Country.Sweden, "SEK " }
        };

        public static decimal ConvertPrice(decimal basePrice, Country targetCountry)
        {
            // Convert from USD to target currency
            return basePrice * ExchangeRates[targetCountry];
        }

        public static string FormatCurrency(decimal amount, Country country)
        {
            decimal convertedAmount = ConvertPrice(amount, country);
            return $"{CurrencySymbols[country]}{convertedAmount:N2}";
        }
    }
    public static class DateTimeExtensions
    {
        public static double TotalMonths(this TimeSpan timeSpan)
        {
            return timeSpan.TotalDays / 30.44; // Average days in a month
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<AssetUser> AssetUsers { get; set; } // Navigation property, many--to-many
    }

    public class AssetUser
    {
        public int AssetId { get; set; } // secondary key
        public Asset Asset { get; set; } // Navigation property

        public int UserId { get; set; } // secondary key
        public User User { get; set; } // Navigation property
    }

    class Program
    {
        static AssetManagementService assetService = new AssetManagementService(); // create static assetService object from class AssetManagementService

        static void Main(string[] args) // entry point for the application 
        {
            while (true) // main menu loop
            {
                DisplayMainMenu();
                string choice = Console.ReadLine();
                Console.OutputEncoding = System.Text.Encoding.UTF8; // used to show euro sign U+20AC 

                switch (choice)
                {
                    case "1":
                        AddAssetMenu();
                        break;
                    case "2":
                        ViewAssetsMenu();
                        break;
                    case "3":
                        UpdateAssetMenu();
                        break;
                    case "4":
                        DeleteAssetMenu();
                        break;
                    case "5":
                        ViewAssetsByOfficeMenu();
                        break;
                    case "6":
                        ViewAssetsByUserMenu();
                        break;
                    case "7":
                        ViewUsersAndAssetsMenu();
                        break;
                    case "8":
                        GenerateStatisticsReport();
                        break;
                    case "9":
                        Console.WriteLine("Exiting the program...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("=== Asset Management System ===");
            Console.WriteLine("1. Add New Asset");
            Console.WriteLine("2. View All Assets");
            Console.WriteLine("3. Update Asset Price");
            Console.WriteLine("4. Delete Asset");
            Console.WriteLine("5. View Assets by Office");
            Console.WriteLine("6. View Assets by User");
            Console.WriteLine("7. View Users and Their Assigned Assets");
            Console.WriteLine("8. Generate Statistics Report");
            Console.WriteLine("9. Exit");
            Console.Write("Enter your choice: ");
        }

        static void AddAssetMenu()
        {
            Console.WriteLine("\n--- Add New Asset ---");
            Console.WriteLine("Select Asset Type:");
            Console.WriteLine("1. MacBook");
            Console.WriteLine("2. Asus Laptop");
            Console.WriteLine("3. Lenovo Laptop");
            Console.WriteLine("4. iPhone");
            Console.WriteLine("5. Samsung Phone");
            Console.WriteLine("6. Nokia Phone");
            Console.Write("Enter your choice: ");

            string typeChoice = Console.ReadLine();
            Asset newAsset = null;

            try // handle exception
            {
                switch (typeChoice)
                {
                    case "1":
                        newAsset = CreateMacBook();
                        break;
                    case "2":
                        newAsset = CreateAsusLaptop();
                        break;
                    case "3":
                        newAsset = CreateLenovoLaptop();
                        break;
                    case "4":
                        newAsset = CreateIphone();
                        break;
                    case "5":
                        newAsset = CreateSamsungPhone();
                        break;
                    case "6":
                        newAsset = CreateNokiaPhone();
                        break;
                    default:
                        Console.WriteLine("Invalid asset type.");
                        return;
                }

                // Add the asset based on its type
                switch (newAsset)
                {
                    case MacBook macBook:
                        assetService.AddAsset(macBook);
                        break;
                    case AsusLaptop asusLaptop:
                        assetService.AddAsset(asusLaptop);
                        break;
                    case LenovoLaptop lenovoLaptop:
                        assetService.AddAsset(lenovoLaptop);
                        break;
                    case Iphone iPhone:
                        assetService.AddAsset(iPhone);
                        break;
                    case SamsungPhone samsungPhone:
                        assetService.AddAsset(samsungPhone);
                        break;
                    case NokiaPhone nokiaPhone:
                        assetService.AddAsset(nokiaPhone);
                        break;
                }

                // Retrieve offices using the asset service's context
                using (var context = new AssetTrackingContext())
                {
                    var offices = context.Offices.ToList();

                    // After creating the asset, prompt for office assignment
                    Console.WriteLine("\nSelect Office:");
                    for (int i = 0; i < offices.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {offices[i].Name} ({offices[i].Country})");
                    }
                    Console.Write("Enter office number: ");
                    if (int.TryParse(Console.ReadLine(), out int officeIndex) &&
                        officeIndex > 0 && officeIndex <= offices.Count)
                    {
                        var selectedOffice = offices[officeIndex - 1];
                        newAsset.OfficeId = selectedOffice.Id;

                        // Update the asset with the office assignment
                        switch (newAsset)
                        {
                            case MacBook macBook:
                                assetService.UpdateAsset(macBook);
                                break;
                            case AsusLaptop asusLaptop:
                                assetService.UpdateAsset(asusLaptop);
                                break;
                            case LenovoLaptop lenovoLaptop:
                                assetService.UpdateAsset(lenovoLaptop);
                                break;
                            case Iphone iPhone:
                                assetService.UpdateAsset(iPhone);
                                break;
                            case SamsungPhone samsungPhone:
                                assetService.UpdateAsset(samsungPhone);
                                break;
                            case NokiaPhone nokiaPhone:
                                assetService.UpdateAsset(nokiaPhone);
                                break;
                        }

                        // Display price in different currencies
                        Console.WriteLine($"Price in USD: ${newAsset.Price}");
                        Console.WriteLine($"Price in EURO: {CurrencyConverter.FormatCurrency(newAsset.Price, Country.Germany)}");
                        Console.WriteLine($"Price in SEK: {CurrencyConverter.FormatCurrency(newAsset.Price, Country.Sweden)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding asset: {ex.Message}");
            }
        }

        static void ViewAssetsMenu()
        {
            assetService.DisplayAllAssets();
        }

        static void UpdateAssetMenu()
        {
            Console.WriteLine("\n--- Update Asset ---");
            assetService.DisplayAllAssets();

            Console.Write("Enter the ID of the asset to update: ");
            if (int.TryParse(Console.ReadLine(), out int assetId))
            {
                var assets = assetService.GetAllAssetsSorted();
                var assetToUpdate = assets.FirstOrDefault(a => a.Id == assetId);

                if (assetToUpdate != null)
                {
                    Console.Write("Enter new $ price: ");
                    if (decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                    {
                        assetToUpdate.Price = newPrice;

                        // Update based on specific type
                        switch (assetToUpdate)
                        {
                            case MacBook macBook:
                                assetService.UpdateAsset(macBook);
                                break;
                            case AsusLaptop asusLaptop:
                                assetService.UpdateAsset(asusLaptop);
                                break;
                            case LenovoLaptop lenovoLaptop:
                                assetService.UpdateAsset(lenovoLaptop);
                                break;
                            case Iphone iPhone:
                                assetService.UpdateAsset(iPhone);
                                break;
                            case SamsungPhone samsungPhone:
                                assetService.UpdateAsset(samsungPhone);
                                break;
                            case NokiaPhone nokiaPhone:
                                assetService.UpdateAsset(nokiaPhone);
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid $ price entered.");
                    }
                }
                else
                {
                    Console.WriteLine("Asset not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID entered.");
            }
        }

        static void DeleteAssetMenu()
        {
            Console.WriteLine("\n--- Delete Asset ---");
            assetService.DisplayAllAssets();

            Console.Write("Enter the ID of the asset to delete: ");
            if (int.TryParse(Console.ReadLine(), out int assetId))
            {
                var assets = assetService.GetAllAssetsSorted();
                var assetToDelete = assets.FirstOrDefault(a => a.Id == assetId);

                if (assetToDelete != null)
                {
                    switch (assetToDelete)
                    {
                        case MacBook macBook:
                            assetService.DeleteAsset<MacBook>(assetId);
                            break;
                        case AsusLaptop asusLaptop:
                            assetService.DeleteAsset<AsusLaptop>(assetId);
                            break;
                        case LenovoLaptop lenovoLaptop:
                            assetService.DeleteAsset<LenovoLaptop>(assetId);
                            break;
                        case Iphone iPhone:
                            assetService.DeleteAsset<Iphone>(assetId);
                            break;
                        case SamsungPhone samsungPhone:
                            assetService.DeleteAsset<SamsungPhone>(assetId);
                            break;
                        case NokiaPhone nokiaPhone:
                            assetService.DeleteAsset<NokiaPhone>(assetId);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Asset not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID entered.");
            }
        }

        static void ViewAssetsByOfficeMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            using (var context = new AssetTrackingContext())
            {
                // Retrieve and display offices
                var offices = context.Offices.ToList();

                Console.WriteLine("\n--- Select Office to View Assets ---");
                for (int i = 0; i < offices.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {offices[i].Name} ({offices[i].Country})");
                }
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Enter office number: ");

                if (int.TryParse(Console.ReadLine(), out int officeChoice) &&
                    officeChoice > 0 && officeChoice <= offices.Count)
                {
                    var selectedOffice = offices[officeChoice - 1];

                    // Retrieve and sort assets for the selected office
                    var officeAssets = context.Set<Asset>()
                        .Where(a => a.OfficeId == selectedOffice.Id)
                        .OrderBy(a => a.PurchaseDate)
                        .ToList();

                    Console.WriteLine($"\nAssets for {selectedOffice.Name} ({selectedOffice.Country}):");
                    Console.WriteLine(new string('=', 118));

                    if (officeAssets.Count == 0)
                    {
                        Console.WriteLine("No assets assigned to this office.");
                    }
                    else
                    {
                        // Print header
                        Console.WriteLine("{0,-3} {1,-15} {2,-10} {3,-15} {4,-12} {5,12} {6,12} {7,15} {8,15}",
                            "ID", "Type", "Brand", "Model", "Purchase Date", "$ Price", "End of Life", "Status", "Local Currency");
                        Console.WriteLine(new string('-', 118));

                        foreach (var asset in officeAssets)
                        {
                            // Set color based on life status
                            switch (asset.LifeStatus)
                            {
                                case AssetLifeStatus.Red:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case AssetLifeStatus.Yellow:
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case AssetLifeStatus.Grey:
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    break;
                                default:
                                    Console.ResetColor();
                                    break;
                            }

                            // Convert price to local currency
                            string localPrice = CurrencyConverter.FormatCurrency(asset.Price, selectedOffice.Country);

                            // Display asset details
                            Console.WriteLine("{0,3} {1,-15} {2,-10} {3,-15} {4,-12} {5, 12:C} {6, 12:d} {7,15} {8, 15:C}",
                                asset.Id,
                                asset.GetType().Name,
                                asset.Manufacturer,
                                asset.Model,
                                asset.PurchaseDate.ToString("d"),
                                asset.Price,
                                asset.EndOfLifeDate,
                                asset.LifeStatus == AssetLifeStatus.Grey ? "Expired" :
                                    (asset.LifeStatus == AssetLifeStatus.Red ? "Near End" :
                                    (asset.LifeStatus == AssetLifeStatus.Yellow ? "Soon Expiring" : "Active")),
                                localPrice);

                            // Reset color
                            Console.ResetColor();
                        }

                        // Print total count
                        Console.WriteLine("\nTotal Assets: {0}", officeAssets.Count);
                    }
                }
                else if (officeChoice != 0)
                {
                    Console.WriteLine("Invalid office selection.");
                }
            }
        }

        // Method to view assets by user
        static void ViewAssetsByUserMenu()
        {
            try
            {
                using (var context = new AssetTrackingContext())
                {
                    // Retrieve and display users
                    List<User> users;
                    try
                    {
                        users = context.Set<User>().ToList();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving users: {ex.Message}");
                        return;
                    }

                    if (users == null || users.Count == 0)
                    {
                        Console.WriteLine("No users found in the system.");
                        return;
                    }

                    Console.WriteLine("\n--- Select User to View Assets ---");
                    for (int i = 0; i < users.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {users[i].Name}");
                    }
                    Console.WriteLine("0. Return to Main Menu");
                    Console.Write("Enter user number: ");

                    // Input validation with exception handling
                    int userChoice;
                    try
                    {
                        if (!int.TryParse(Console.ReadLine(), out userChoice))
                        {
                            throw new FormatException("Invalid input. Please enter a valid number.");
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }

                    // User selection validation
                    if (userChoice == 0)
                    {
                        return;
                    }

                    if (userChoice < 1 || userChoice > users.Count)
                    {
                        Console.WriteLine("Invalid user selection.");
                        return;
                    }

                    var selectedUser = users[userChoice - 1];

                    // Retrieve and display assets for the selected user
                    List<Asset> userAssets;
                    try
                    {
                        userAssets = assetService.GetAssetsByUser(selectedUser.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error retrieving assets for user {selectedUser.Name}: {ex.Message}");
                        return;
                    }

                    Console.WriteLine($"\nAssets assigned to {selectedUser.Name}:");
                    Console.WriteLine(new string('-', 118));

                    if (userAssets == null || userAssets.Count == 0)
                    {
                        Console.WriteLine("No assets assigned to this user.");
                        return;
                    }

                    try
                    {
                        // Print header
                        Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15} {7,-15}",
                            "ID", "Type", "Manufacturer", "Model", "Purchase Date", "$ Price", "End of Life", "Near End of Life");
                        Console.WriteLine(new string('-', 118));

                        foreach (var asset in userAssets)
                        {
                            try
                            {
                                // Set color based on life status
                                switch (asset.LifeStatus)
                                {
                                    case AssetLifeStatus.Red:
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        break;
                                    case AssetLifeStatus.Yellow:
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        break;
                                    case AssetLifeStatus.Grey:
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        break;
                                    default:
                                        Console.ResetColor();
                                        break;
                                }

                                // Display asset details
                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15:d} {7,-15}",
                                    asset.Id,
                                    asset.GetType().Name,
                                    asset.Manufacturer,
                                    asset.Model,
                                    asset.PurchaseDate.ToString("d"),
                                    asset.Price,
                                    asset.EndOfLifeDate,
                                    asset.LifeStatus == AssetLifeStatus.Grey ? "Expired" :
                                        (asset.LifeStatus == AssetLifeStatus.Red ? "Near End" :
                                        (asset.LifeStatus == AssetLifeStatus.Yellow ? "Soon Expiring" : "Active")));

                                // Reset color
                                Console.ResetColor();
                            }
                            catch (Exception assetDisplayEx)
                            {
                                Console.WriteLine($"Error displaying asset: {assetDisplayEx.Message}");
                            }
                        }

                        // Print total asset count
                        Console.WriteLine("\nTotal Assets: {0}", userAssets.Count);
                    }
                    catch (Exception displayEx)
                    {
                        Console.WriteLine($"Error displaying assets: {displayEx.Message}");
                    }
                }
            }
            catch (InvalidOperationException dbEx)
            {
                Console.WriteLine($"Database operation error: {dbEx.Message}");
                Console.WriteLine("Possible reasons:");
                Console.WriteLine("- Database connection issue");
                Console.WriteLine("- Database not initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // New method to display all users and their assigned assets
        static void ViewUsersAndAssetsMenu()
        {
            try
            {
                using (var context = new AssetTrackingContext())
                {
                    // Retrieve all users
                    var users = context.Set<User>().ToList();

                    if (users == null || users.Count == 0)
                    {
                        Console.WriteLine("No users found in the system.");
                        return;
                    }

                    Console.WriteLine("\n--- Users and Their Assigned Assets ---");
                    Console.WriteLine(new string('-', 50));

                    foreach (var user in users)
                    {
                        try
                        {
                            // Retrieve assets for the current user
                            var userAssets = assetService.GetAssetsByUser(user.Id);

                            Console.WriteLine($"User: {user.Name}");
                            if (userAssets == null || userAssets.Count == 0)
                            {
                                Console.WriteLine("  No assets assigned.");
                            }
                            else
                            {
                                Console.WriteLine("  Assigned Assets:");
                                foreach (var asset in userAssets)
                                {
                                    try
                                    {
                                        Console.WriteLine($"    - {asset.GetType().Name}: {asset.Manufacturer} {asset.Model}");
                                    }
                                    catch (Exception assetEx)
                                    {
                                        Console.WriteLine($"    - Error displaying asset: {assetEx.Message}");
                                    }
                                }
                            }
                            Console.WriteLine(new string('-', 50));
                        }
                        catch (Exception userAssetEx)
                        {
                            Console.WriteLine($"Error retrieving assets for user {user.Name}: {userAssetEx.Message}");
                        }
                    }
                }
            }
            catch (InvalidOperationException dbEx)
            {
                Console.WriteLine($"Database operation error: {dbEx.Message}");
                Console.WriteLine("Possible reasons:");
                Console.WriteLine("- Database connection issue");
                Console.WriteLine("- Database not initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        // Helper methods to create specific asset types
        static MacBook CreateMacBook()
        {
            return new MacBook
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                ProcessorType = PromptInput("Processor Type"),
                MemoryGB = PromptInteger("Memory (GB)")
            };
        }

        static AsusLaptop CreateAsusLaptop()
        {
            return new AsusLaptop
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                ProcessorType = PromptInput("Processor Type"),
                MemoryGB = PromptInteger("Memory (GB)")
            };
        }

        static LenovoLaptop CreateLenovoLaptop()
        {
            return new LenovoLaptop
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                ProcessorType = PromptInput("Processor Type"),
                MemoryGB = PromptInteger("Memory (GB)")
            };
        }

        static Iphone CreateIphone()
        {
            return new Iphone
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                StorageCapacity = PromptInput("Storage Capacity"),
                Color = PromptInput("Color")
            };
        }

        static SamsungPhone CreateSamsungPhone()
        {
            return new SamsungPhone
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                StorageCapacity = PromptInput("Storage Capacity"),
                Color = PromptInput("Color")
            };
        }

        static NokiaPhone CreateNokiaPhone()
        {
            return new NokiaPhone
            {
                Manufacturer = PromptInput("Manufacturer"),
                Model = PromptInput("Model"),
                PurchaseDate = PromptDate("Purchase Date"),
                Price = PromptDecimal("Price"),
                StorageCapacity = PromptInput("Storage Capacity"),
                Color = PromptInput("Color")
            };
        }

        // Utility input methods
        static string PromptInput(string prompt)
        {
            Console.Write($"Enter {prompt}: ");
            return Console.ReadLine();
        }

        static DateTime PromptDate(string prompt)
        {
            while (true)
            {
                Console.Write($"Enter {prompt} (yyyy-MM-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
                {
                    return result;
                }
                Console.WriteLine("Invalid date format. Please try again.");
            }
        }

        static decimal PromptDecimal(string prompt)
        {
            while (true)
            {
                Console.Write($"Enter {prompt}: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        static int PromptInteger(string prompt)
        {
            while (true)
            {
                Console.Write($"Enter {prompt}: ");
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }

        // Method to generate statistics report
        static void GenerateStatisticsReport()
        {
            using (var context = new AssetTrackingContext())
            {
                try
                {
                    Console.WriteLine("\n=== Asset Statistics Report ===");

                    // Total Assets
                    var totalAssets = context.Set<Asset>().Count();
                    Console.WriteLine($"Total Number of Assets: {totalAssets}");

                    // Retrieve all assets into memory
                    var allAssets = context.Set<Asset>().ToList();

                    // Assets by Type (in-memory grouping)
                    var assetsByType = allAssets
                        .GroupBy(a => a.GetType().Name) // Group by the type name
                        .Select(g => new
                        {
                            Type = g.Key,
                            Count = g.Count()
                        })
                        .OrderByDescending(x => x.Count)
                        .ToList();

                    Console.WriteLine("\nAssets by Type:");
                    foreach (var typeCount in assetsByType)
                    {
                        Console.WriteLine($"{typeCount.Type}: {typeCount.Count}");
                    }

                    // Total Asset Value
                    var totalAssetValue = allAssets.Sum(a => a.Price);
                    Console.WriteLine($"\nTotal Asset Value: ${totalAssetValue:N2}");

                    // Assets by Office
                    var assetsByOffice = context.Offices
                        .Select(o => new
                        {
                            OfficeName = o.Name,
                            AssetCount = o.Assets.Count,
                            TotalValue = o.Assets.Sum(a => a.Price) // No need for ?? 0 here
                        })
                        .OrderByDescending(x => x.AssetCount)
                        .ToList();

                    Console.WriteLine("\nAssets by Office:");
                    foreach (var officeStats in assetsByOffice)
                    {
                        Console.WriteLine($"{officeStats.OfficeName}: {officeStats.AssetCount} assets, Total Value: ${officeStats.TotalValue:N2}");
                    }

                    // Near End of Life Assets
                    var nearEndOfLifeAssets = allAssets.Count(a => a.IsNearEndOfLife);
                    Console.WriteLine($"\nAssets Near End of Life: {nearEndOfLifeAssets}");

                    // Oldest and Newest Assets
                    var oldestAsset = allAssets.OrderBy(a => a.PurchaseDate).FirstOrDefault();
                    var newestAsset = allAssets.OrderByDescending(a => a.PurchaseDate).FirstOrDefault();

                    if (oldestAsset != null)
                    {
                        Console.WriteLine($"\nOldest Asset: {oldestAsset.Manufacturer} {oldestAsset.Model} (Purchased on {oldestAsset.PurchaseDate:d})");
                    }
                    else
                    {
                        Console.WriteLine("\nNo assets found.");
                    }

                    if (newestAsset != null)
                    {
                        Console.WriteLine($"Newest Asset: {newestAsset.Manufacturer} {newestAsset.Model} (Purchased on {newestAsset.PurchaseDate:d})");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while generating the report: {ex.Message}");
                }
            }
        }
    }
}