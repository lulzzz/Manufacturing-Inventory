using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Application.UseCases {
    public interface IUseCase<T,P> {
        Task<P> Execute(T input);
    }

    public interface IUseCaseV2<T, P> {
        Task<P> ExecuteAsync(T input);
        P Execute(T input);
    }
}
