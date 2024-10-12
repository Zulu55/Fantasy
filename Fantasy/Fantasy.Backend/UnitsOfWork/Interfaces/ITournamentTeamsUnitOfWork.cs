using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Interfaces;

public interface ITournamentTeamsUnitOfWork
{
    Task<IEnumerable<TournamentTeam>> GetComboAsync(int tournamentId);

    Task<ActionResponse<TournamentTeam>> AddAsync(TournamentTeamDTO tournamentTeamDTO);

    Task<ActionResponse<IEnumerable<TournamentTeam>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}