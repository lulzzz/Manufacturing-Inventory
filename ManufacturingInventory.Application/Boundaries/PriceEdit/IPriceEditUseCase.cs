﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Application.Boundaries.PriceEdit {
    public interface IPriceEditUseCase:IUseCase<PriceEditInput,PriceEditOutput> {
        Task<IEnumerable<Distributor>> GetDistributors();
        Task<Price> GetPrice(int priceId);
        Task Load();

    }
}