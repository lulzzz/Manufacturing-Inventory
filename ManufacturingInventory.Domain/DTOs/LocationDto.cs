using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Domain.DTOs {
    public class LocationDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefualt { get; set; }
        public bool ShowParts { get; set; }
        public LocationType LocationType{get;set;}
        public IEnumerable<InstanceDto> PartInstances { get; set; }
        public IEnumerable<PartDto> Parts { get; set; }
        public IEnumerable<TransactionDTO> Transactions { get; set; }

        public LocationDto() {
            this.Id = -1;
            this.Name = "N/A";
            this.Description = "N/A";
            this.IsDefualt = false;
            this.LocationType = LocationType.NotSelected;
            this.PartInstances = new List<InstanceDto>();
            this.Parts = new List<PartDto>();
            this.Transactions = new List<TransactionDTO>();
            this.ShowParts = false;
        }

        public LocationDto(Location location) {
            if (location != null) {
                this.Id = location.Id;
                this.Name = location.Name;
                this.Description = location.Description;
                this.IsDefualt = location.IsDefualt;
                Type type = location.GetType();
                if (type == typeof(Warehouse)) {
                    var warehouse = (Warehouse)location;
                    this.PartInstances = warehouse.ItemsAtLocation.Select(instance => new InstanceDto(instance));
                    this.Parts = warehouse.StoredParts.Select(part => new PartDto(part));
                    this.LocationType = LocationType.Warehouse;
                    this.Transactions = warehouse.Transactions.Select(transaction => new TransactionDTO(transaction));
                    this.ShowParts = true;
                } else if (type == typeof(Consumer)) {
                    var consumer = (Consumer)location;
                    this.PartInstances = consumer.ItemsAtLocation.Select(instance => new InstanceDto(instance));
                    this.Parts = new List<PartDto>();
                    this.Transactions = consumer.Transactions.Select(transaction => new TransactionDTO(transaction));
                    this.ShowParts = false;
                    this.LocationType = LocationType.Consumer;
                } else {
                    this.Id = -1;
                    this.Name = "N/A";
                    this.LocationType = LocationType.NotSelected;
                    this.Description = "N/A";
                    this.IsDefualt = false;
                    this.ShowParts = false;
                    this.Parts = new List<PartDto>();
                    this.PartInstances = new List<InstanceDto>();
                    this.Transactions = new List<TransactionDTO>();
                }
            } else {
                this.Id = -1;
                this.Name = "N/A";
                this.Description = "N/A";
                this.IsDefualt = false;
                this.ShowParts = false;
                this.Parts = new List<PartDto>();
                this.PartInstances = new List<InstanceDto>();
                this.Transactions = new List<TransactionDTO>();
                this.LocationType = LocationType.NotSelected;
            }
        }
    }
}
