using ManufacturingInventory.Common.Data;
using ManufacturingInventory.Common.Model;
using ManufacturingInventory.Common.Model.Entities;
using ManufacturingInventory.Common.Model.DbContextExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Common.Buisness.Interfaces;

namespace ManufacturingInventory.PartsManagment.Internal {
    public class PartManagmentService : IPartManagerService {

        public ManufacturingContext Context { get; private set; }

        public IUserService UserService { get; private set; }

        public IEntityService<Part> PartService { get; private set; }
        public IEntityService<PartInstance> PartInstanceService { get; private set; }
        public IEntityService<Location> LocationService { get; private set; }
        public IEntityService<Condition> ConditionService { get; private set; }
        //public IEntityService<Usage> UsageService { get; private set; }
       //public IEntityService<Organization> OrganizationService { get; private set; }
        public IEntityService<Transaction> TransactionService { get; private set; }

        public PartManagmentService(ManufacturingContext context,IUserService userService) {
            this.Context = context;
            this.UserService = userService;
            this.PartService = new PartService(context);
            this.PartInstanceService = new PartInstanceService(context);
            this.LocationService = new LocationService(context);
            this.ConditionService = new ConditionService(context);
            this.TransactionService = new TransactionService(context);
        }

        public bool CheckIn(Transaction transaction) => throw new NotImplementedException();
        public Task<bool> CheckInAsync(Transaction transaction) => throw new NotImplementedException();

        public bool BatchCheckOut(IList<Transaction> transactions) => throw new NotImplementedException();

        public async Task<bool> BatchCheckOutAsync(IList<Transaction> transactions) {
            bool success = true;
            List<Task> tasks = new List<Task>();
            foreach(var transaction in transactions) {
                transaction.SessionId = this.UserService.CurrentSession.Id;
                success = success & await this.CheckOutAsync(transaction, true);
            }
            return success;
            //if (success) {
            //    return await this.SaveChangesAsync(true);
            //} else {
            //    await this.UndoChangesAsync();
            //    return false;
            //}
        }

        public bool CheckOut(Transaction transaction) => throw new NotImplementedException();

        public async Task<bool> CheckOutAsync(Transaction transaction,bool save) {
            var partInstance = await this.PartInstanceService.GetEntityAsync(e => e.Id == transaction.PartInstance.Id,true);
            var location =await this.LocationService.GetEntityAsync(e => e.Id == transaction.Location.Id, true);
            if(partInstance!=null && location != null) {
                partInstance.LocationId = location.Id;
                if (partInstance.IsBubbler) {
                    partInstance.CostReported = false;
                    partInstance.Quantity = 0;
                }
                var checkOut=await this.TransactionService.AddAsync(transaction, false);
                if (checkOut != null) {
                    if (save) {
                        return await this.SaveChangesAsync();
                    } else {
                        return true;
                    }
                } else {
                    return false;
                }
            } else {
                return false;
            }

        }

        public bool DeletePart(Part part, bool save) => throw new NotImplementedException();
        public Task<bool> DeletePartAsync(Part part, bool save) => throw new NotImplementedException();

        public void LoadAll() {
            this.PartInstanceService.Load();
            this.LocationService.Load();
            this.ConditionService.Load();
            this.TransactionService.Load();
            this.PartService.Load();
        }

        public async Task LoadAllAsync() {
            await this.PartInstanceService.LoadAsync();
            await this.LocationService.LoadAsync();
            await this.ConditionService.LoadAsync();
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
            try {
                if (this.Context.ChangeTracker.HasChanges()) {
                    await this.Context.SaveChangesAsync();
                }
                return true;
            } catch(Exception e) {
                return false;
            }
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
    }
}
