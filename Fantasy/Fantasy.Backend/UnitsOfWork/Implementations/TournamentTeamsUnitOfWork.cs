using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations;

public class TournamentTeamsUnitOfWork : GenericUnitOfWork<TournamentTeam>, ITournamentTeamsUnitOfWork
{
    private readonly ITournamentTeamsRepository _tournamentTeamsRepository;

    public TournamentTeamsUnitOfWork(IGenericRepository<TournamentTeam> repository, ITournamentTeamsRepository tournamentTeamsRepository) : base(repository)
    {
        _tournamentTeamsRepository = tournamentTeamsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<TournamentTeam>>> GetAsync(PaginationDTO pagination) => await _tournamentTeamsRepository.GetAsync(pagination);

    public async Task<ActionResponse<TournamentTeam>> AddAsync(TournamentTeamDTO tournamentTeamDTO) => await _tournamentTeamsRepository.AddAsync(tournamentTeamDTO);

    public async Task<IEnumerable<TournamentTeam>> GetComboAsync(int tournamentId) => await _tournamentTeamsRepository.GetComboAsync(tournamentId);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _tournamentTeamsRepository.GetTotalRecordsAsync(pagination);
}