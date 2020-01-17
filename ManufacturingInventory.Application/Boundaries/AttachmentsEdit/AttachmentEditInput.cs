using ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit {

    


    public class AttachmentPartEditInput : IAttachmentInput<Part> {
        public AttachmentPartEditInput(Part entity, bool isNew) {
            this.Entity = entity;
            this.IsNew = isNew;
        }

        public Part Entity { get; set; }
        public bool IsNew { get; set; }
    }

    public class AttachmentPartInstanceEditInput : IAttachmentInput<PartInstance> {
        public AttachmentPartInstanceEditInput(PartInstance entity, bool isNew) {
            this.Entity = entity;
            this.IsNew = isNew;
        }

        public PartInstance Entity { get; set; }
        public bool IsNew { get; set; }
    }

    public class AttachmentPriceEditInput : IAttachmentInput<Price> {
        public AttachmentPriceEditInput(Price entity, bool isNew) {
            this.Entity = entity;
            this.IsNew = isNew;
        }

        public Price Entity { get; set; }
        public bool IsNew { get; set; }
    }

    public class AttachmentDistributorEditInput : IAttachmentInput<Distributor> {
        public AttachmentDistributorEditInput(Distributor entity, bool isNew) {
            this.Entity = entity;
            this.IsNew = isNew;
        }

        public Distributor Entity { get; set; }
        public bool IsNew { get; set; }
    }

    public class AttachmentManufacturerEditInput : IAttachmentInput<Manufacturer> {
        public AttachmentManufacturerEditInput(Manufacturer entity, bool isNew) {
            this.Entity = entity;
            this.IsNew = isNew;
        }

        public Manufacturer Entity { get; set; }
        public bool IsNew { get; set; }
    }
}
