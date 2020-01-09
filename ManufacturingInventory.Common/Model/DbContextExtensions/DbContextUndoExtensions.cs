using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Common.Model.DbContextExtensions {
    public static class DbContextUndoExtensions {
        public static void UndoDbContext(this DbContext context) {
            if (context == null)
                throw new ArgumentNullException();

            foreach(var entry in context.ChangeTracker.Entries()) {
                switch (entry.State) {
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default:break;
                }
            }
        }

        public static void UndoDbEntries<T>(this DbContext context) where T : class {
            if (context == null) {
                throw new ArgumentNullException();
            }

            foreach (var entry in context.ChangeTracker.Entries<T>()) {
                switch (entry.State) {

                    case EntityState.Added: {
                        entry.State = EntityState.Detached;
                        break;
                    }

                    case EntityState.Deleted: {
                        entry.Reload();
                        break;
                    }
                    case EntityState.Modified: {
                        entry.State = EntityState.Unchanged;
                        break;
                    }
                    default: break;
                }
            }
        }

        public static void UndoDbEntry(this DbContext context, object entity) {
            if (context == null || entity == null) {
                throw new ArgumentNullException();
            }
            var entry = context.Entry(entity);
            switch (entry.State) {

                case EntityState.Added: {
                    entry.State = EntityState.Detached;
                    break;
                }

                case EntityState.Deleted: {
                    entry.Reload();
                    break;
                }

                case EntityState.Modified: {
                    entry.State = EntityState.Unchanged;
                    break;
                }

                default: break;
            }

        }

        public static void UndoDbEntityProperty(this DbContext context, object entity, string propertyName) {
            if (context == null || entity == null || propertyName == null) {
                throw new ArgumentException();
            }

            try {
                var entry = context.Entry(entity);
                if (entry.State == EntityState.Added || entry.State == EntityState.Detached) {
                    return;
                }
                object propertyValue = entry.OriginalValues.GetValue<object>(propertyName);
                entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;
            } catch {
                throw;
            }
        }
    }
}
