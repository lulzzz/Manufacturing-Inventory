using ManufacturingInventory.Infrastructure.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace ManufacturingInventory.Infrastructure.Model {
    public class ManufacturingContext : DbContext {

        public DbSet<Part> Parts { get; set; }
        public DbSet<PartInstance> PartInstances { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Distributor> Distributors { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<UserAlert> UserAlerts { get; set; }
        public DbSet<PartManufacturer> PartManufacturers { get; set; }
        public DbSet<BubblerParameter> BubblerParameters { get; set; }
        public DbSet<PartPrice> PartPrices { get; set; }
        public DbSet<PriceLog> PriceLogs { get; set; }

        public ManufacturingContext(DbContextOptions<ManufacturingContext> options) : base(options) {
            this.ChangeTracker.LazyLoadingEnabled = false;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
            //this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public ManufacturingContext() : base() {
            this.ChangeTracker.LazyLoadingEnabled = false;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
            //this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies(false);
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.EnableDetailedErrors(true);

            optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.UseSqlServer("server=DESKTOP-NGE4P2E;database=manufacturing_inventory;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.UseSqlServer("server=DESKTOP-LJJI4KF;database=manufacturing_inventory;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.UseSqlServer(Microsoft.Extensions.Configuration.GetConnectionString("FacilityConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Organization>().HasBaseType<Category>();
            builder.Entity<Condition>().HasBaseType<Category>();
            builder.Entity<Usage>().HasBaseType<Category>();
            builder.Entity<StockType>().HasBaseType<Category>();
            //builder.Entity<Designation>().HasBaseType<Category>();

            builder.Entity<Warehouse>().HasBaseType<Location>();
            builder.Entity<Consumer>().HasBaseType<Location>();

            #region Concurrency

            builder.Entity<Part>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<PartInstance>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Alert>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Attachment>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Category>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Contact>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Distributor>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Location>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Manufacturer>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Parameter>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Unit>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<InstanceParameter>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<PartManufacturer>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Permission>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Session>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Price>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<Transaction>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<User>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<UserAlert>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<PartPrice>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            builder.Entity<PriceLog>()
                .Property(e => e.RowVersion)
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();

            #endregion

            #region Parts

            builder.Entity<Part>()
                .HasOne(e => e.Organization)
                .WithMany(e => e.Parts)
                .IsRequired(false)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<Part>()
            //    .HasOne(e => e.Usage)
            //    .WithMany(e => e.Parts)
            //    .HasForeignKey(e => e.UsageId)
            //    .IsRequired(false)
            //    .HasForeignKey(e => e.UsageId);

            builder.Entity<Part>()
                .HasOne(e => e.Warehouse)
                .WithMany(e => e.StoredParts)
                .IsRequired(false)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Attachment>()
                .HasOne(e => e.Manufacturer)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.ManufacturerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region ConfigureAttachment

            builder.Entity<Attachment>()
                    .HasOne(e => e.Part)
                    .WithMany(e => e.Attachments)
                    .HasForeignKey(e => e.PartId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Attachment>()
                .HasOne(e => e.Distributor)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.DistributorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Attachment>()
                .HasOne(e => e.Price)
                .WithOne(e => e.Attachment)
                .HasForeignKey<Attachment>(e => e.PriceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Attachment>()
                .HasOne(e => e.PartInstance)
                .WithMany(e => e.Attachments)
                .HasForeignKey(e => e.PartInstanceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region ConfigurePartInstance

            builder.Entity<PartInstance>()
                .HasOne(e => e.Part)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.PartId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasOne(e => e.StockType)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.StockTypeId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasOne(e => e.Usage)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.UsageId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasOne(e => e.Condition)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.ConditionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasOne(e => e.CurrentLocation)
                .WithMany(e => e.ItemsAtLocation)
                .HasForeignKey(e => e.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);



            #endregion

            #region Price_Distributer_Instance

            //builder.Entity<Price>()
            //    .HasKey(e => new { e.DistributorId, e.PartInstanceId });

            //builder.Entity<Price>()
            //    .HasOne(e => e.PartInstance)
            //    .WithOne(e => e.Price)
            //    .IsRequired(false)
            //    .HasForeignKey<Price>(e => e.PartInstanceId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<Price>()
            //    .HasOne(e => e.Distributor)
            //    .WithMany(e => e.Prices)
            //    .IsRequired(true)
            //    .HasForeignKey(e => e.DistributorId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<PartInstance>()
            //    .HasOne(e => e.Price)
            //    .with(e => e.PartInstances)
            //    .HasForeignKey(e => e.PriceId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.Entity<PartInstance>()
            //    .HasOne(e => e.Price)
            //    .(e => e.PartInstances)
            //    .IsRequired(false)
            //    .HasForeignKey<Price>(e => e.PartInstanceId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //ParInstance-Price/Distributor

            builder.Entity<PartPrice>()
                .HasKey(pm => new { pm.PartId, pm.PriceId });

            builder.Entity<PriceLog>()
                .HasKey(pm => new { pm.PriceId, pm.PartInstanceId });

            builder.Entity<Part>()
                .HasMany(e => e.PartPrices)
                .WithOne(e => e.Part)
                .HasForeignKey(e => e.PartId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Price>()
                .HasMany(e => e.PartPrices)
                .WithOne(e => e.Price)
                .IsRequired(true)
                .HasForeignKey(e => e.PriceId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasOne(e => e.Price)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.PriceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartInstance>()
                .HasMany(e => e.PriceLogs)
                .WithOne(e => e.PartInstance)
                .HasForeignKey(e => e.PartInstanceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Price>()
                .HasMany(e => e.PriceLogs)
                .WithOne(e => e.Price)
                .HasForeignKey(e => e.PriceId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Distributor>()
                .HasMany(e => e.Prices)
                .WithOne(e => e.Distributor)
                .HasForeignKey(e => e.DistributorId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Distributor>()
                .HasMany(e => e.Contacts)
                .WithOne(e => e.Distributor)
                .HasForeignKey(e => e.DistributorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Parameters



            builder.Entity<Parameter>()
                .HasOne(e => e.Unit)
                .WithMany(e => e.Parameters)
                .HasForeignKey(e => e.UnitId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BubblerParameter>()
                .HasOne(e => e.PartInstance)
                .WithOne(e => e.BubblerParameter)
                .HasForeignKey<PartInstance>(e => e.BubblerParameterId)
                .IsRequired(false);

            #endregion

            #region Manufacturer 

            builder.Entity<Manufacturer>()
                .HasMany(e => e.Contacts)
                .WithOne(e => e.Manufacturer)
                .HasForeignKey(e => e.ManufacturerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartManufacturer>()
                .HasKey(pm => new { pm.PartId, pm.ManufacturerId });

            builder.Entity<PartManufacturer>()
                .HasOne(e => e.Manufacturer)
                .WithMany(e => e.PartManufacturers)
                .HasForeignKey(e => e.ManufacturerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PartManufacturer>()
                .HasOne(e => e.Part)
                .WithMany(e => e.PartManufacturers)
                .HasForeignKey(e => e.PartId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Transactions

            builder.Entity<Transaction>()
                .HasOne(e => e.Session)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.SessionId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Transaction>()
                .HasOne(e => e.Location)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.LocationId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Transaction>()
                .HasOne(e => e.ReferenceTransaction)
                .WithOne()
                .HasForeignKey<Transaction>(e => e.ReferenceTransactionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region User

            builder.Entity<UserAlert>()
                .HasKey(ua => new { ua.UserId, ua.AlertId });

            builder.Entity<UserAlert>()
                .HasOne(e => e.Alert)
                .WithMany(e => e.UserAlerts)
                .HasForeignKey(e => e.AlertId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserAlert>()
                .HasOne(e => e.User)
                .WithMany(e => e.UserAlerts)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .HasMany(e => e.Sessions)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .HasOne(e => e.Permission)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.PermissionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Session>()
                .HasOne(e => e.User)
                .WithMany(e => e.Sessions)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            this.Seed(builder);
        }

        private void Seed(ModelBuilder builder) {
            this.SeedPermissions(builder);
            this.SeedDistributors(builder);
            this.SeedCategories(builder);
            this.SeedLocations(builder);
            this.SeedManufacturers(builder);
        }

        private void SeedPermissions(ModelBuilder builder) {
            builder.Entity<Permission>()
                .HasData(new Permission() {
                    Id = 1,
                    Name = "InventoryAdminAccount",
                    Description = "Full Inventory Privileges and User Control"
                });

            builder.Entity<Permission>()
                .HasData(new Permission() {
                    Id = 2,
                    Name = "InventoryUserAccount",
                    Description = "Inventory View Only"
                });

            builder.Entity<Permission>()
                .HasData(new Permission() {
                    Id = 3,
                    Name = "InventoryUserFullAccount",
                    Description = "Full Inventory Privileges"
                });

            builder.Entity<Permission>()
                .HasData(new Permission() {
                    Id = 4,
                    Name = "InventoryUserLimitedAccount",
                    Description = "Inventory Check In/Check Out/Create"
                });

            builder.Entity<User>()
                .HasData(new User() {
                    Id = 1,
                    PermissionId = 1,
                    FirstName = "Andrew",
                    LastName = "Elmendorf",
                    UserName = "AElmendo"
                });
        }

        private void SeedDistributors(ModelBuilder builder) {

            builder.Entity<Distributor>().HasData(new Distributor {Id=1, Name = "LSP Industrial Ceramics Inc.", Description = "Boron Nitride Parts" });
            builder.Entity<Distributor>().HasData(new Distributor { Id = 2, Name = "Rayotek", Description = "Sapphire Parts" });
            builder.Entity<Distributor>().HasData(new Distributor { Id = 3, Name = "Mersen", Description = "SiC & TaC Coated Graphite parts" });
            builder.Entity<Distributor>().HasData(new Distributor { Id = 4, Name = "Aixtron", Description = "Original Aixtron Parts" });
            builder.Entity<Distributor>().HasData(new Distributor { Id = 5, Name = "Quality Quartz Engineering ", Description = "All Quartz Parts" });
            builder.Entity<Distributor>().HasData(new Distributor { Id = 6, Name = "Akzo Nobel", Description = "" });
        }

        private void SeedCategories(ModelBuilder builder) {
            builder.Entity<Condition>().HasData(new Condition { Id = 1, Name = "New", Description = "A new Part", IsDefault = true }); ;
            builder.Entity<Condition>().HasData(new Condition { Id = 2, Name = "Used", Description = "Part that has been used and returned to inventory" , IsDefault = false });
            builder.Entity<Condition>().HasData(new Condition { Id = 3, Name = "Need Cleaning", Description = "A part returned to inventory and needs cleaning. i.e. Satellites", IsDefault = false });
            builder.Entity<Condition>().HasData(new Condition { Id = 4, Name = "Needs Repair", Description = "A part returned to inventory in need of repair/refurbish" , IsDefault = false });
            builder.Entity<Condition>().HasData(new Condition { Id = 5, Name = "Refurbished", Description = "A part in inventory that was repaired/refurbished", IsDefault = false });
            builder.Entity<Condition>().HasData(new Condition { Id = 6, Name = "Depleted", Description = "A part's stock is depleted. No additional stock will be added or returned",IsDefault = false });

            builder.Entity<Organization>().HasData(new Organization { Id = 7, Name = "Raw Materials", Description = "", IsDefault = false });
            builder.Entity<Organization>().HasData(new Organization { Id = 8, Name = "Supplies", Description = "", IsDefault = false });

            builder.Entity<Usage>().HasData(new Usage { Id = 9, Name = "All Systems", Description = "Used on all Epi Systems", IsDefault = false });
            builder.Entity<Usage>().HasData(new Usage { Id = 10, Name = "Growth", Description = "General Growth Usage", IsDefault = true });
            builder.Entity<Usage>().HasData(new Usage { Id = 11, Name = "A Systems", Description = "Used on A Systems", IsDefault = false });
            builder.Entity<Usage>().HasData(new Usage { Id = 12, Name = "B Systems", Description = "Used on B Systems", IsDefault = false });
            builder.Entity<Usage>().HasData(new Usage { Id = 13, Name = "C Systems", Description = "Used on C Systems", IsDefault = false });
            builder.Entity<StockType>().HasData(new StockType { Id = 14, Name = "Individual", Description = "Individual Stock", MinQuantity = 0, SafeQuantity = 0, IsDefault = true });

        }

        private void SeedLocations(ModelBuilder builder) {
            builder.Entity<Warehouse>().HasData(new Warehouse { Id = 1, Name = "Epi System Parts", Description = "" });
            builder.Entity<Warehouse>().HasData(new Warehouse { Id = 2, Name = "Gas Bay", Description = "" });
            builder.Entity<Warehouse>().HasData(new Warehouse { Id = 3, Name = "Epi Chase", Description = "" });
            builder.Entity<Warehouse>().HasData(new Warehouse { Id = 4, Name = "Process Lab", Description = "" });
            builder.Entity<Warehouse>().HasData(new Warehouse { Id = 5, Name = "Back Warehouse", Description = "" });

            builder.Entity<Consumer>().HasData(new Consumer { Id = 6, Name = "System B01", Description = "Reactor B01" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 7, Name = "System B02", Description = "Reactor B02" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 8, Name = "System B03", Description = "Reactor B03" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 9, Name = "System B04", Description = "Reactor B04" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 10, Name = "System B05", Description = "Reactor B05" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 11, Name = "System B06", Description = "Reactor B06" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 12, Name = "System B07", Description = "Reactor B07" });

            builder.Entity<Consumer>().HasData(new Consumer { Id = 13, Name = "System A01", Description = "Reactor A01" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 14, Name = "System A02", Description = "Reactor A02" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 15, Name = "System A03", Description = "Reactor A03" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 16, Name = "System A04", Description = "Reactor A04" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 17, Name = "System A05", Description = "Reactor A05" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 18, Name = "System A06", Description = "Reactor A06" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 19, Name = "System A07", Description = "Reactor A07" });

            builder.Entity<Consumer>().HasData(new Consumer { Id = 20, Name = "System C01", Description = "Reactor C01" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 21, Name = "System C02", Description = "Reactor C02" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 22, Name = "System C03", Description = "Reactor C03" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 23, Name = "System C04", Description = "Reactor C04" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 24, Name = "System C05", Description = "Reactor C05" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 25, Name = "System C06", Description = "Reactor C06" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 26, Name = "System C07", Description = "Reactor C07" });

            builder.Entity<Consumer>().HasData(new Consumer { Id = 27, Name = "System C08", Description = "Reactor C08" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 28, Name = "System C09", Description = "Reactor C09" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 29, Name = "System C10", Description = "Reactor C10" });
            builder.Entity<Consumer>().HasData(new Consumer { Id = 30, Name = "System C11", Description = "Reactor C11" });

            builder.Entity<Consumer>().HasData(new Consumer { Id = 31, Name = "Epi Process", Description = "Generic Consumer for cost reporting" });
        }

        private void SeedManufacturers(ModelBuilder builder) {
            builder.Entity<Manufacturer>().HasData(new Manufacturer { Id = 1, Name = "Mersen", Description = "Mersen deals with all SiC coated & TaC coated graphite parts." });
        }

    }
}
