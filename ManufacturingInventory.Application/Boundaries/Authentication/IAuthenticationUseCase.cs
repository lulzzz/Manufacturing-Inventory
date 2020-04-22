using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.Boundaries.Authentication {
    public interface IAuthenticationUseCase:IUseCase<AuthenticationInput,AuthenticationOutput> {
        Task<User> GetUser(string userName);
    }
}
