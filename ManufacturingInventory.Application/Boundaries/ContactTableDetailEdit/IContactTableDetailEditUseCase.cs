using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Domain.DTOs;

namespace ManufacturingInventory.Application.Boundaries.ContactTableDetailEdit {
    public interface IContactTableDetailEditUseCase:IUseCase<ContactTableDetailEditInput,ContactTableDetailEditOutput> {
        Task<IEnumerable<ContactDTO>> GetContacts(int distributorId);
        Task<ContactDTO> GetContact(int contactId);
        Task Load();
    }
}
