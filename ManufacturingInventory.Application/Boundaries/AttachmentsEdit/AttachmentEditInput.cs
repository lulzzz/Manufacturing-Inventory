using ManufacturingInventory.Domain.Enums;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit {
    public enum AttachmentOperation {
        OPEN,
        DOWNLOAD,
        RENAME,
        DELETE,
        NEW
    }

    public class AttachmentEditInput {
        
        public AttachmentEditInput(string name, string displayName, string description, string sourceReference, 
            AttachmentOperation operation,GetAttachmentBy by, int entityId) {
            this.Name = name;
            this.DisplayName = displayName;
            this.Description = description;
            this.SourceReference = sourceReference;

            this.AttachmentOperation = operation;
            this.AttachmentBy = by;
            this.EntityId = entityId;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string SourceReference { get; set; }
        public string FileReference { get; set; }
        public string Extension { get; set; }
        public bool Expires { get; set; }
        public AttachmentOperation AttachmentOperation { get; set; }
        public GetAttachmentBy AttachmentBy { get; set; }
        public int EntityId { get; set; }

    }
}
