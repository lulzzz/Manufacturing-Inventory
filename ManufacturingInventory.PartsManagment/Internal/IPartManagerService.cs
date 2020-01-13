using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Buisness.Interfaces;
using ManufacturingInventory.Common.Data;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;

namespace ManufacturingInventory.PartsManagment.Internal {


    public interface IPartManagerService {

        ManufacturingContext Context { get; }
        IUserService UserService { get; }

        IEntityService<Part> PartService { get; }
        IEntityService<PartInstance> PartInstanceService { get; }
        IEntityService<Location> LocationService { get; }
        IEntityService<Condition> ConditionService { get; }
        //IEntityService<Usage> UsageService { get; }
        //IEntityService<Organization> OrganizationService { get; }
        //IEntityService<Attachment> AttachmentService { get; }
        IEntityService<Transaction> TransactionService { get; }
        //IEntityService<Price> PriceService { get; }

        bool BatchCheckOut(IList<Transaction> transactions);
        Task<bool> BatchCheckOutAsync(IList<Transaction> transactions);

        bool CheckIn(Transaction transaction);
        Task<bool> CheckInAsync(Transaction transaction);

        bool CheckOut(Transaction transaction);
        Task<bool> CheckOutAsync(Transaction transaction,bool save);

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
