using Fantasy.Backend.Data;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
{
    private readonly DataContext _context;

    public CountriesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<Country>> GetAsync(int id)
    {
        var country = await _context.Countries
             .Include(x => x.Teams)
             .FirstOrDefaultAsync(x => x.Id == id);

        if (country == null)
        {
            return new ActionResponse<Country>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Country>
        {
            WasSuccess = true,
            Result = country
        };
    }

    public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync()
    {
        var countries = await _context.Countries
            .Include(x => x.Teams)
            .ToListAsync();
        return new ActionResponse<IEnumerable<Country>>
        {
            WasSuccess = true,
            Result = countries
        };
    }

    public async Task<IEnumerable<Country>> GetComboAsync()
    {
        return await _context.Countries
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
}