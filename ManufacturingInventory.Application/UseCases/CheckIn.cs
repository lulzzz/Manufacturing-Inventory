using ManufacturingInventory.Infrastructure.Model;
using ManufacturingInventory.Infrastructure.Model.Entities;
using ManufacturingInventory.Infrastructure.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManufacturingInventory.Application.UseCases {
    public class CheckIn {
        private IRepository<Transaction> _transactionRepository;
        private IRepository<Location> _locationRepository;
        private IRepository<Category> _categoryRepository;
        private IUnitOfWork _unitOfWork;

        public CheckIn(
            IRepository<Transaction> transactionRepository, 

            IRepository<Location> locationRepository, 
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork) {
            this._transactionRepository = transactionRepository;
            this._locationRepository = locationRepository;
            this._categoryRepository = categoryRepository;
            this._unitOfWork = unitOfWork;
        }

        
    }
}
