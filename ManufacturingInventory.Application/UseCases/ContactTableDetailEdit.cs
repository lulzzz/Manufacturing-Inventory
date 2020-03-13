using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;

namespace ManufacturingInventory.Application.UseCases {
    public class ContactTableDetailEdit : IContactTableDetailEditUseCase {
        private ManufacturingContext _context;
        private IRepository<Contact> _contactRepository;
        private IRepository<Distributor> _distributorRepository;
        private IUnitOfWork _unitOfWork;

        public ContactTableDetailEdit(ManufacturingContext context) {
            this._context = context;
            this._contactRepository = new ContactRepository(context);
            this._distributorRepository = new DistributorRepository(context);
            this._unitOfWork = new UnitOfWork(context);
        }

        public async Task<ContactTableDetailEditOutput> Execute(ContactTableDetailEditInput input) {
            switch (input.EditAction) {
                case Boundaries.EditAction.Add:
                    return await this.ExecuteNew(input);
                case Boundaries.EditAction.Delete:
                    return await this.ExecuteDelete(input);
                case Boundaries.EditAction.Update:
                    return await this.ExecuteUpdate(input);
                default:
                    return new ContactTableDetailEditOutput(null, false, "Error: No Such Edit Option");
            }
        }


        private async Task<ContactTableDetailEditOutput> ExecuteNew(ContactTableDetailEditInput input) {
            var contact = new Contact(input.Contact);
            contact.DistributorId = input.DistributorId;
            var added=await this._contactRepository.AddAsync(contact);
            if (added != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new ContactTableDetailEditOutput(added, true, "Success: Contact Added");
                } else {
                    await this._unitOfWork.Undo();
                    return new ContactTableDetailEditOutput(null, false, "Error: Save Failed,Please Contact Admin");
                }
            } else {
                await this._unitOfWork.Undo();
                return new ContactTableDetailEditOutput(null, false, "Error: Could Not Add Contact");
            }
        }

        private async Task<ContactTableDetailEditOutput> ExecuteUpdate(ContactTableDetailEditInput input) {
            var contact = await this._contactRepository.GetEntityAsync(e => e.Id == input.Contact.Id);
            contact.Set(input.Contact);
            var updated = await this._contactRepository.UpdateAsync(contact);
            if (updated != null) {
                var count = await this._unitOfWork.Save();
                if (count > 0) {
                    return new ContactTableDetailEditOutput(updated, true, "Success: Contact Updated");
                } else {
                    await this._unitOfWork.Undo();
                    return new ContactTableDetailEditOutput(null, false, "Error: Save Failed,Please Contact Admin");
                }
            } else {
                await this._unitOfWork.Undo();
                return new ContactTableDetailEditOutput(null, false, "Error: Could Not Update Contact,Please Contact Admin");
            }
        }

        private async Task<ContactTableDetailEditOutput> ExecuteDelete(ContactTableDetailEditInput input) {
            return new ContactTableDetailEditOutput(null, false, "Error:Delete Option Not Implemented Yet");
        }

        public async Task<ContactDTO> GetContact(int contactId) {
            var contact = await this._contactRepository.GetEntityAsync(e => e.Id == contactId);
            return new ContactDTO(contact);
        }

        public async Task<IEnumerable<ContactDTO>> GetContacts(int distributorId) {
            return (await this._contactRepository.GetEntityListAsync(e => e.DistributorId == distributorId)).Select(contact => new ContactDTO(contact));
        }

        public async Task Load() {
            await this._contactRepository.LoadAsync();
            await this._distributorRepository.LoadAsync();
        }
    }
}
