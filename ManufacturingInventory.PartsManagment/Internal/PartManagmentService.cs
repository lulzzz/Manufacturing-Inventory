using ManufacturingInventory.Common.Data;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.Common.Model.DbContextExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Buisness.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingInventory.PartsManagment.Internal {


    public class PartManagmentService : IPartManagerService {

        public ManufacturingContext Context { get; private set; }

        public IUserService UserService { get; private set; }

        public IEntityService<Part> PartService { get; private set; }
        public IEntityService<PartInstance> PartInstanceService { get; private set; }
        public IEntityService<Location> LocationService { get; private set; }
        public IEntityService<Category> CategoryService { get; private set; }
        //public IEntityService<Usage> UsageService { get; private set; }
       //public IEntityService<Organization> OrganizationService { get; private set; }
        public IEntityService<Transaction> TransactionService { get; private set; }

        public PartManagmentService(ManufacturingContext context,IUserService userService) {
            this.Context = context;
            this.UserService = userService;
            this.PartService = new PartService(context);
            this.PartInstanceService = new PartInstanceService(context);
            this.LocationService = new LocationService(context);
            this.CategoryService = new CategoryService(context);
            this.TransactionService = new TransactionService(context);
        }


        public PartManagmentResponce CheckOut(IList<Transaction> transactions, bool isBubbler) => throw new NotImplementedException();

        public async Task<PartManagmentResponce> CheckOutAsync(IList<Transaction> transactions,bool isBubbler) {
            
            List<Task> tasks = new List<Task>();
            StringBuilder succeeded = new StringBuilder();
            StringBuilder failed = new StringBuilder();
            bool success = true;
            failed.AppendLine("Failed Items: ");
            succeeded.AppendLine("Successful Items: ");
            Category used;
            if (isBubbler) {
                used = await this.CategoryService.GetEntityAsync(e => e.Name == "Used", false);
            } else {
                used = null;
            }
            foreach (var transaction in transactions) {
                this.ConfigureTransaction(transaction, transaction.InventoryAction,(Condition)used);
                var val=await this.Context.SaveChangesAsync();
                if (val>0) {
                    succeeded.AppendFormat("PartInstance: {0}", transaction.PartInstance.Name).AppendLine();
                } else {
                    success = false;
                    failed.AppendFormat("PartInstance: {0}", transaction.PartInstance.Name).AppendLine();
                }
            }
            succeeded.AppendLine(failed.ToString());
            return new PartManagmentResponce(success, succeeded.ToString());
        }


        private void ConfigureTransaction(Transaction transaction,InventoryAction action,Condition condition=null) {
            transaction.SessionId = this.UserService.CurrentSession.Id;
            if (transaction.PartInstance.IsBubbler) {
                transaction.PartInstance.UpdateQuantity(-1);
                transaction.PartInstance.CostReported = false;           
            } else {
                transaction.PartInstance.UpdateQuantity(0 - transaction.Quantity);
            }
            transaction.PartInstance.CurrentLocation = transaction.Location;
            if (condition != null) {
                transaction.PartInstance.Condition = condition;
                var entry = this.Context.Entry<Condition>(condition);
                if (entry.State == EntityState.Detached) {
                    entry.State = EntityState.Modified;
                }
            }
            var locEntry=this.Context.Entry<Location>(transaction.Location);
            if (locEntry.State == EntityState.Detached) {
                locEntry.State = EntityState.Modified;
            }
            var instanceEntry=this.Context.Entry<PartInstance>(transaction.PartInstance);
            if (instanceEntry.State == EntityState.Detached) {
                instanceEntry.State = EntityState.Modified;
            }
            if (transaction.PartInstance.IsBubbler) {
                //this.Context.Update(transaction.PartInstance.BubblerParameter);
                var bubblerEntry=this.Context.Entry<BubblerParameter>(transaction.PartInstance.BubblerParameter);
                if (bubblerEntry.State == EntityState.Detached) {
                    bubblerEntry.State = EntityState.Modified;
                }
            }

            this.Context.AddAsync<Transaction>(transaction);
        }

        public bool DeletePart(Part part, bool save) => throw new NotImplementedException();
        public Task<bool> DeletePartAsync(Part part, bool save) => throw new NotImplementedException();

        public void LoadAll() {
            this.PartInstanceService.Load();
            this.LocationService.Load();
            this.CategoryService.Load();
            this.TransactionService.Load();
            this.PartService.Load();
        }

        public async Task LoadAllAsync() {
            await this.PartInstanceService.LoadAsync();
            await this.LocationService.LoadAsync();
            await this.CategoryService.LoadAsync();
            await this.TransactionService.LoadAsync();
            await this.PartService.LoadAsync();
        }

        public void UndoChanges() {
            this.Context.UndoDbContext();
        }

        public async Task UndoChangesAsync() {
            await Task.Run(() => {
                this.Context.UndoDbContext();
            });
        }

        public async Task<bool> SaveChangesAsync(bool undoIfFail) {
            try {
                if (this.Context.ChangeTracker.HasChanges()) {
                    await this.Context.SaveChangesAsync();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this.Context.UndoDbContext();

                return false;
            }
    }

        public bool SaveChanges(bool undoIfFail) {
            try {
                if (this.Context.ChangeTracker.HasChanges()) {
                    this.Context.SaveChanges();
                }
                return true;
            } catch {
                if (undoIfFail)
                    this.Context.UndoDbContext();

                return false;
            }
        }

        public async Task<bool> SaveChangesAsync() {
            await this.Context.SaveChangesAsync();
            return true;
        }

        public bool SaveChanges() {
            try {
                if (this.Context.ChangeTracker.HasChanges()) {
                    this.Context.SaveChanges();
                }
                return true;
            } catch {
                return false;
            }
        }

        public PartManagmentResponce CheckIn(Transaction transaction) => throw new NotImplementedException();
        public Task<PartManagmentResponce> CheckInAsync(Transaction transaction) => throw new NotImplementedException();
    }
}
