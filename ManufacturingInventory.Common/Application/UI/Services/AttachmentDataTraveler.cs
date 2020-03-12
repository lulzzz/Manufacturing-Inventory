using ManufacturingInventory.Domain.Enums;

namespace ManufacturingInventory.Common.Application.UI.Services {
    public class AttachmentDataTraveler {

        public AttachmentDataTraveler(GetAttachmentBy getBy,int entityId) {
            this.EntityId = entityId;
            this.GetBy = getBy;
        }

        public int EntityId { get; set; }
        public GetAttachmentBy GetBy {get;set;}
    }

}
