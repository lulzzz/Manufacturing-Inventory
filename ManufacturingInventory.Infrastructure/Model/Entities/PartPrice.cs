using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Infrastructure.Model.Entities {
    public class PartPrice {

        public PartPrice() {

        }

        public PartPrice(Part part,Price price) {
            this.Price = price;
            this.Part = part;
        }

        public PartPrice(int partId,int priceId) {
            this.PriceId = priceId;
            this.PartId = priceId;
        }

        public int Id { get; set; }
        public byte[] RowVersion { get; set; }

        public int PartId { get; set; }
        public Part Part { get; set; }

        public int PriceId { get; set; }
        public Price Price { get; set; }

        public void Set(PartPrice partPrice) {
            this.PartId = partPrice.PartId;
            this.PriceId = partPrice.PriceId;
        }

    }
}
