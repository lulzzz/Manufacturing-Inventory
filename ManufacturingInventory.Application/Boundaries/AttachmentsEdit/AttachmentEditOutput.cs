using ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit {
    public class AttachmentPartEditOutput :  IAttachmentEditOutput<Part> {
        public AttachmentPartEditOutput(Part entity, bool isNew, bool success, string message) {
            this.Entity = entity;
            this.IsNew = isNew;
            this.Success = success;
            this.Message = message;
        }

        public Part Entity { get; set; }
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AttachmentPartInstanceEditOutput : IAttachmentEditOutput<PartInstance> {
        public AttachmentPartInstanceEditOutput(PartInstance entity, bool isNew, bool success, string message) {
            this.Entity = entity;
            this.IsNew = isNew;
            this.Success = success;
            this.Message = message;
        }

        public PartInstance Entity { get; set; }
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AttachmentPriceEditOutput : IAttachmentEditOutput<Price> {
        public AttachmentPriceEditOutput(Price entity, bool isNew, bool success, string message) {
            this.Entity = entity;
            this.IsNew = isNew;
            this.Success = success;
            this.Message = message;
        }

        public Price Entity { get; set; }
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AttachmentDistributorEditOutput : IAttachmentEditOutput<Distributor> {
        public AttachmentDistributorEditOutput(Distributor entity, bool isNew, bool success, string message) {
            this.Entity = entity;
            this.IsNew = isNew;
            this.Success = success;
            this.Message = message;
        }

        public Distributor Entity { get; set; }
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class AttachmentManufacturerEditOutput : IAttachmentEditOutput<Manufacturer> {
        public AttachmentManufacturerEditOutput(Manufacturer entity, bool isNew, bool success, string message) {
            this.Entity = entity;
            this.IsNew = isNew;
            this.Success = success;
            this.Message = message;
        }

        public Manufacturer Entity { get; set; }
        public bool IsNew { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
