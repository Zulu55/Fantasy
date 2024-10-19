using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations
{
    public class MatchesUnitOfWork : GenericUnitOfWork<Match>, IMatchesUnitOfWork
    {
        private readonly IMatchesRepository _matchesRepository;

        public MatchesUnitOfWork(IGenericRepository<Match> repository, IMatchesRepository matchesRepository) : base(repository)
        {
            _matchesRepository = matchesRepository;
        }

        public override async Task<ActionResponse<Match>> GetAsync(int id) => await _matchesRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Match>>> GetAsync(PaginationDTO pagination) => await _matchesRepository.GetAsync(pagination);

        public async Task<ActionResponse<Match>> AddAsync(MatchDTO matchDTO) => await _matchesRepository.AddAsync(matchDTO);

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _matchesRepository.GetTotalRecordsAsync(pagination);

        public async Task<ActionResponse<Match>> UpdateAsync(MatchDTO matchDTO) => await _matchesRepository.UpdateAsync(matchDTO);
    }
}