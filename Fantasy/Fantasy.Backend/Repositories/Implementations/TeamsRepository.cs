using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations
{
    public class TeamsRepository : GenericRepository<Team>, ITeamsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public TeamsRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync()
        {
            var teams = await _context.Teams
                .Include(x => x.Country)
                .OrderBy(x => x.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<Team>>
            {
                WasSuccess = true,
                Result = teams
            };
        }

        public override async Task<ActionResponse<Team>> GetAsync(int id)
        {
            var team = await _context.Teams
                 .Include(x => x.Country)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (team == null)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Team>
            {
                WasSuccess = true,
                Result = team
            };
        }

        public override async Task<ActionResponse<IEnumerable<Team>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Teams
                .Include(x => x.Country)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Team>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            var queryable = _context.Teams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Country!.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<Team>> AddAsync(TeamDTO teamDTO)
        {
            var country = await _context.Countries.FindAsync(teamDTO.CountryId);
            if (country == null)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR004"
                };
            }

            var team = new Team
            {
                Country = country,
                Name = teamDTO.Name,
            };

            if (!string.IsNullOrEmpty(teamDTO.Image))
            {
                var imageBase64 = Convert.FromBase64String(teamDTO.Image!);
                team.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "teams");
            }

            _context.Add(team);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Team>
                {
                    WasSuccess = true,
                    Result = team
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        public async Task<IEnumerable<Team>> GetComboAsync(int countryId)
        {
            return await _context.Teams
                .Where(x => x.CountryId == countryId)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<ActionResponse<Team>> UpdateAsync(TeamDTO teamDTO)
        {
            var currentTeam = await _context.Teams.FindAsync(teamDTO.Id);
            if (currentTeam == null)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR005"
                };
            }

            var country = await _context.Countries.FindAsync(teamDTO.CountryId);
            if (country == null)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR004"
                };
            }

            if (!string.IsNullOrEmpty(teamDTO.Image))
            {
                var imageBase64 = Convert.FromBase64String(teamDTO.Image!);
                currentTeam.Image = await _fileStorage.SaveFileAsync(imageBase64, ".jpg", "teams");
            }

            currentTeam.Country = country;
            currentTeam.Name = teamDTO.Name;

            _context.Update(currentTeam);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Team>
                {
                    WasSuccess = true,
                    Result = currentTeam
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Team>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
    }
}