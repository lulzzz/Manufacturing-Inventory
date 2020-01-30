using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit {
    public class AttachmentEditOutput:IOutput   {
        public AttachmentEditOutput(Attachment entity, bool success, string message) {
            this.Attachment = entity;
            this.Success = success;
            this.Message = message;
        }

        public Attachment Attachment { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
