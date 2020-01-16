using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Buisness.Interfaces;
using ManufacturingInventory.Common.Data;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;

namespace ManufacturingInventory.PartsManagment.Internal {

    public class PartManagmentResponce {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        public PartManagmentResponce(bool success,string message) {
            this.Success = success;
            this.Message = message;
        }
    }

    public interface IPartManagerService {

        ManufacturingContext Context { get; }
        IUserService UserService { get; }

        IEntityService<Part> PartService { get; }
        IEntityService<PartInstance> PartInstanceService { get; }
        IEntityService<Location> LocationService { get; }
        IEntityService<Category> CategoryService { get; }
        //IEntityService<Attachment> AttachmentService { get; }
        IEntityService<Transaction> TransactionService { get; }

        PartManagmentResponce CheckOut(IList<Transaction> transactions, bool isBubbler);
        Task<PartManagmentResponce> CheckOutAsync(IList<Transaction> transactions,bool isBubbler);

        PartManagmentResponce CheckIn(Transaction transaction);
        Task<PartManagmentResponce> CheckInAsync(Transaction transaction);

        bool DeletePart(Part part,bool save);
        Task<bool> DeletePartAsync(Part part, bool save);

        void LoadAll();
        Task LoadAllAsync();

        void UndoChanges();
        Task UndoChangesAsync();

        bool SaveChanges();
        Task<bool> SaveChangesAsync();

        bool SaveChanges(bool undoIfFail);
        Task<bool> SaveChangesAsync(bool undoIfFail);

    }
}
