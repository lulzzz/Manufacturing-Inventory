namespace ManufacturingInventory.Application.Boundaries.AttachmentsEdit.Interfaces {
    public interface IAttachmentInput<T>: IInput {
        T Entity { get; set; }
    }
}
