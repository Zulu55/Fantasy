using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Interfaces;

public interface IPredictionsUnitOfWork
{
    Task<ActionResponse<Prediction>> AddAsync(PredictionDTO predictionDTO);

    Task<ActionResponse<Prediction>> UpdateAsync(PredictionDTO predictionDTO);

    Task<ActionResponse<Prediction>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Prediction>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}