using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ManufacturingInventory.Application.UseCases;
using ManufacturingInventory.Domain.DTOs;
using ManufacturingInventory.Infrastructure.Model.Entities;

namespace ManufacturingInventory.Application.Boundaries.ReportingBoundaries {
    public interface IReportingUseCase:IUseCase<ReportingInput,ReportingOutput> {
        Task Load();
    }
}
