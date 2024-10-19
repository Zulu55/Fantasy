using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface IMatchesRepository
{
    Task<ActionResponse<Match>> AddAsync(MatchDTO matchDTO);

    Task<ActionResponse<Match>> UpdateAsync(MatchDTO matchDTO);

    Task<ActionResponse<Match>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Match>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}