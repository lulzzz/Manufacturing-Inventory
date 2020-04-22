using ManufacturingInventory.Domain.Security.Interfaces;
using ManufacturingInventory.Infrastructure.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.Boundaries.Authentication {
    public class AuthenticationOutput : IOutput {

        public AuthenticationOutput(IUserService userService,bool success,string message) {
            this.UserService = userService;
            this.Success = success;
            this.Message = message;
        }

        public IUserService UserService { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
