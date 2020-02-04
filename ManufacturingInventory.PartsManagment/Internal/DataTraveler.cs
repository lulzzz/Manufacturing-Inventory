
using ManufacturingInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.PartsManagment.Internal {
    public class DataTraveler {
        public int PartId { get; set; }
        public bool HoldsBubblers { get; set; }
        public bool IsNew { get; set; }
        public bool IsEdit { get; set; }
    }

    public class AttachmentDataTraveler {

        public AttachmentDataTraveler(GetAttachmentBy getBy,int entityId) {
            this.EntityId = entityId;
            this.GetBy = getBy;
        }

        public int EntityId { get; set; }
        public GetAttachmentBy GetBy {get;set;}
    }

    public class PriceDataTraveler {

        public PriceDataTraveler(int partId, bool isNew, int? priceId=null, int? instanceId=null) {
            this.PriceId = priceId;
            this.InstanceId = instanceId;
            this.PartId = partId;
            this.IsNew = isNew;
        }

        public int? PriceId { get; set; }
        public int? InstanceId { get; set; }
        public int PartId { get; set; }
        public bool IsNew { get; set; }
    }
}
