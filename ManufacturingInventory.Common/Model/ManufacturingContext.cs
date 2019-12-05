using System.Collections.Generic;
using ManufacturingInventory.Common.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.Common.Model {
    public class ManufacturingContext:DbContext {

        public DbSet<Part> Parts { get; set; }
        public DbSet<PartInstance> PartInstances { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Distributor> Distributor { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<InstanceParameter> InstanceParameters { get; set; }
        public DbSet<Price> Price { get; set; }
        public DbSet<Session> Session { get; set; }




        public ManufacturingContext(DbContextOptions<ManufacturingContext> options) : base(options) {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public ManufacturingContext() : base() {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies(false);
            optionsBuilder.UseSqlServer("server=172.20.4.20;database=facilitymonitoring;User Id=aelmendorf;Password=Drizzle123!;");
            //optionsBuilder.UseSqlServer("server=172.20.4.20;database=monitoring_dev;Trusted_Connection=True;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer(Microsoft.Extensions.Configuration.GetConnectionString("FacilityConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Organization>().HasBaseType<Category>();
            builder.Entity<Condition>().HasBaseType<Category>();
            builder.Entity<Usage>().HasBaseType<Category>();
            builder.Entity<PartType>().HasBaseType<PartType>();

            builder.Entity<Warehouse>().HasBaseType<Location>();
            builder.Entity<Consumer>().HasBaseType<Location>();

            builder.Entity<Part>()
                .HasOne(e => e.Organization)
                .WithMany(e => e.Parts)
                .IsRequired(false)
                .HasForeignKey(e => e.OgranizationId);

            builder.Entity<Part>()
                .HasOne(e => e.Usage)
                .WithMany(e => e.Parts)
                .IsRequired(false)
                .HasForeignKey(e => e.UsageId);

            builder.Entity<Part>()
                .HasOne(e => e.Warehouse)
                .WithMany(e => e.StoredParts)
                .IsRequired(false)
                .HasForeignKey(e => e.WarehouseId);

            builder.Entity<PartManufacturer>();



        }
    }
}
