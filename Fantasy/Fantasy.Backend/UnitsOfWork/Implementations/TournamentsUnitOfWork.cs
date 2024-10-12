using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations;

public class TournamentsUnitOfWork : GenericUnitOfWork<Tournament>, ITournamentsUnitOfWork
{
    private readonly ITournamentsRepository _tournamentsRepository;

    public TournamentsUnitOfWork(IGenericRepository<Tournament> repository, ITournamentsRepository tournamentsRepository) : base(repository)
    {
        _tournamentsRepository = tournamentsRepository;
    }

    public override async Task<ActionResponse<Tournament>> GetAsync(int id) => await _tournamentsRepository.GetAsync(id);

    public override async Task<ActionResponse<IEnumerable<Tournament>>> GetAsync(PaginationDTO pagination) => await _tournamentsRepository.GetAsync(pagination);

    public async Task<ActionResponse<Tournament>> AddAsync(TournamentDTO tournamentDTO) => await _tournamentsRepository.AddAsync(tournamentDTO);

    public async Task<IEnumerable<Tournament>> GetComboAsync() => await _tournamentsRepository.GetComboAsync();

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _tournamentsRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<Tournament>> UpdateAsync(TournamentDTO tournamentDTO) => await _tournamentsRepository.UpdateAsync(tournamentDTO);
}