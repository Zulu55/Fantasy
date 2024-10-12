using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations
{
    public class TournamentTeamsRepository : GenericRepository<TournamentTeam>, ITournamentTeamsRepository
    {
        private readonly DataContext _context;

        public TournamentTeamsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<TournamentTeam>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.TournamentTeams
                .Include(x => x.Team)
                .AsQueryable();
            queryable = queryable.Where(x => x.TournamentId == pagination.Id);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Team.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<TournamentTeam>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Team.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<TournamentTeam>> AddAsync(TournamentTeamDTO tournamentTeamDTO)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentTeamDTO.TournamentId);
            if (tournament == null)
            {
                return new ActionResponse<TournamentTeam>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            var team = await _context.Teams.FindAsync(tournamentTeamDTO.TeamId);
            if (team == null)
            {
                return new ActionResponse<TournamentTeam>
                {
                    WasSuccess = false,
                    Message = "ERR005"
                };
            }

            var tournamentTeam = new TournamentTeam
            {
                Tournament = tournament,
                Team = team,
            };

            _context.Add(tournamentTeam);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<TournamentTeam>
                {
                    WasSuccess = true,
                    Result = tournamentTeam
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<TournamentTeam>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<TournamentTeam>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        public async Task<IEnumerable<TournamentTeam>> GetComboAsync(int tournamentId)
        {
            return await _context.TournamentTeams
                .Include(x => x.Team)
                .Where(x => x.TournamentId == tournamentId)
                .OrderBy(x => x.Team.Name)
                .ToListAsync();
        }

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            var queryable = _context.TournamentTeams.AsQueryable();
            queryable = queryable.Where(x => x.TournamentId == pagination.Id);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Team.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }
    }
}