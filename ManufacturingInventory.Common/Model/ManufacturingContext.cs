using System.Collections.Generic;
using ManufacturingInventory.Common.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.Common.Model {
    public class ManufacturingContext:DbContext {

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
        public DbSet<InstanceParameter> InstanceParameters { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<UserAlert> UserAlerts { get; set; }
        public DbSet<PartManufacturer> PartManufacturers { get; set; }

        public ManufacturingContext(DbContextOptions<ManufacturingContext> options) : base(options) {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public ManufacturingContext() : base() {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies(false);
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=manufacturing_inventory;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.UseSqlServer("server=DESKTOP-LJJI4KF;database=manufacturing_inventory;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.
            //optionsBuilder.UseSqlServer("server=172.20.4.20;database=monitoring_dev;Trusted_Connection=True;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer(Microsoft.Extensions.Configuration.GetConnectionString("FacilityConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Organization>().HasBaseType<Category>();
            builder.Entity<Condition>().HasBaseType<Category>();
            builder.Entity<Usage>().HasBaseType<Category>();
            builder.Entity<PartType>().HasBaseType<Category>();

            builder.Entity<Warehouse>().HasBaseType<Location>();
            builder.Entity<Consumer>().HasBaseType<Location>();

            builder.Entity<OutgoingTransaction>().HasBaseType<Transaction>();
            builder.Entity<IncomingTransaction>().HasBaseType<Transaction>();
            builder.Entity<ReturningTransaction>().HasBaseType<Transaction>();

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

            #endregion

            #region Parts

            builder.Entity<Part>()
                .HasOne(e => e.Organization)
                .WithMany(e => e.Parts)
                .IsRequired(false)
                .HasForeignKey(e => e.OgranizationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Part>()
                .HasOne(e => e.Usage)
                .WithMany(e => e.Parts)
                .HasForeignKey(e => e.UsageId)
                .IsRequired(false)
                .HasForeignKey(e => e.UsageId);

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
                    .WithMany(e => e.Attachments)
                    .HasForeignKey(e => e.PriceId)
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
                .HasOne(e => e.PartType)
                .WithMany(e => e.PartInstances)
                .HasForeignKey(e => e.PartTypeId)
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
                .WithMany(e =>e.ItemsAtLocation)
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


            //ParInstance-Price/Distributor
            builder.Entity<PartInstance>()
                .HasOne(e => e.Price)
                .WithOne(e => e.PartInstance)
                .HasForeignKey<Price>(e => e.PartInstanceId)
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

            builder.Entity<InstanceParameter>()
                .HasKey(ip => new { ip.PartInstanceId, ip.ParameterId });


            builder.Entity<InstanceParameter>()
                .HasOne(e => e.Parameter)
                .WithMany(e => e.InstanceParameters)
                .HasForeignKey(e => e.ParameterId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<InstanceParameter>()
                .HasOne(e => e.PartInstance)
                .WithOne(e => e.InstanceParameter)
                .HasForeignKey<InstanceParameter>(e => e.PartInstanceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Parameter>()
                .HasOne(e => e.Unit)
                .WithMany(e => e.Parameters)
                .HasForeignKey(e => e.UnitId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

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

            builder.Entity<OutgoingTransaction>()
                .HasOne(e => e.Consumer)
                .WithMany(e => e.OutgoingTransactions)
                .HasForeignKey(e => e.ConsumerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<IncomingTransaction>()
                .HasOne(e => e.Warehouse)
                .WithMany(e => e.IncomingTransactions)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ReturningTransaction>()
                .HasOne(e => e.OutgoingTransaction)
                .WithOne(e => e.ReturningTransaction)
                .HasForeignKey<ReturningTransaction>(e => e.OutgoingTransactionId)
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
            builder.Entity<Permission>()
                .HasData(new Permission() 
                {
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
                    Id=3,
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
                    Id=1,
                    PermissionId=1,
                    FirstName="Andrew",
                    LastName="Elmendorf",
                    UserName="AElmendo"
                });
        }
    }
}
