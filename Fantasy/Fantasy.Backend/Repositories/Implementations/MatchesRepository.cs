using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations;

public class MatchesRepository : GenericRepository<Match>, IMatchesRepository
{
    private readonly DataContext _context;

    public MatchesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<Match>> AddAsync(MatchDTO matchDTO)
    {
        var tournament = await _context.Tournaments.FindAsync(matchDTO.TournamentId);
        if (tournament == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var local = await _context.Teams.FindAsync(matchDTO.LocalId);
        if (local == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR010"
            };
        }

        var visitor = await _context.Teams.FindAsync(matchDTO.VisitorId);
        if (visitor == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR011"
            };
        }

        var match = new Match
        {
            IsActive = matchDTO.IsActive,
            Date = matchDTO.Date,
            Tournament = tournament,
            Local = local,
            Visitor = visitor
        };

        _context.Add(match);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Match>
            {
                WasSuccess = true,
                Result = match
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Matches.AsQueryable();
        queryable = queryable.Where(x => x.TournamentId == pagination.Id);

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                             x.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        double count = await queryable.CountAsync();
        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = (int)count
        };
    }

    public override async Task<ActionResponse<IEnumerable<Match>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Matches
            .Include(x => x.Tournament)
            .Include(x => x.Local)
            .Include(x => x.Visitor)
            .AsQueryable();
        queryable = queryable.Where(x => x.TournamentId == pagination.Id);

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            queryable = queryable.Where(x => x.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                             x.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        return new ActionResponse<IEnumerable<Match>>
        {
            WasSuccess = true,
            Result = await queryable
                .OrderBy(x => x.IsClosed)
                .ThenBy(x => x.Date)
                .Paginate(pagination)
                .ToListAsync()
        };
    }

    public override async Task<ActionResponse<Match>> GetAsync(int id)
    {
        var team = await _context.Matches
             .Include(x => x.Tournament)
             .Include(x => x.Local)
             .Include(x => x.Visitor)
             .FirstOrDefaultAsync(c => c.Id == id);

        if (team == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<Match>
        {
            WasSuccess = true,
            Result = team
        };
    }

    public async Task<ActionResponse<Match>> UpdateAsync(MatchDTO matchDTO)
    {
        var currentMatch = await _context.Matches.FindAsync(matchDTO.Id);
        if (currentMatch == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR012"
            };
        }

        var tournament = await _context.Tournaments.FindAsync(matchDTO.TournamentId);
        if (tournament == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR009"
            };
        }

        var local = await _context.Teams.FindAsync(matchDTO.LocalId);
        if (local == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR010"
            };
        }

        var visitor = await _context.Teams.FindAsync(matchDTO.VisitorId);
        if (visitor == null)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = "ERR011"
            };
        }

        currentMatch.Local = local;
        currentMatch.Visitor = visitor;
        currentMatch.GoalsVisitor = matchDTO.GoalsVisitor;
        currentMatch.GoalsLocal = matchDTO.GoalsLocal;
        currentMatch.Date = matchDTO.Date;
        currentMatch.IsActive = matchDTO.IsActive;

        _context.Update(currentMatch);
        try
        {
            await _context.SaveChangesAsync();
            //if (currentMatch.GoalsLocal != null && currentMatch.GoalsVisitor != null)
            //{
            //    await CloseMatchAsync(currentMatch);
            //}
            return new ActionResponse<Match>
            {
                WasSuccess = true,
                Result = currentMatch
            };
        }
        catch (Exception exception)
        {
            return new ActionResponse<Match>
            {
                WasSuccess = false,
                Message = exception.Message
            };
        }
    }

    //public async Task CloseMatchAsync(Match match)
    //{
    //    match.IsClosed = true;
    //    _context.Update(match);

    //    var predictions = await _context.Predictions
    //        .Where(x => x.MatchId == match.Id)
    //        .ToListAsync();
    //    foreach (var prediction in predictions)
    //    {
    //        var points = CalculatePoints(match, prediction);
    //        prediction.Points = points;
    //        _context.Update(prediction);
    //    }
    //    await _context.SaveChangesAsync();
    //}

    //public int CalculatePoints(Match match, Prediction prediction)
    //{
    //    int points = 0;
    //    if (prediction.GoalsLocal == null || prediction.GoalsVisitor == null)
    //    {
    //        return points;
    //    }

    //    var matchStatus = GetMatchStatus(match.GoalsLocal!.Value, match.GoalsVisitor!.Value);
    //    var predictionStatus = GetMatchStatus(prediction.GoalsLocal!.Value, prediction.GoalsVisitor!.Value);
    //    if (matchStatus == predictionStatus) points += 5;
    //    if (match.GoalsLocal == prediction.GoalsLocal) points += 2;
    //    if (match.GoalsVisitor == prediction.GoalsVisitor) points += 2;
    //    if (Math.Abs((decimal)match.GoalsLocal! - (decimal)match.GoalsVisitor!) == Math.Abs((decimal)prediction.GoalsLocal! - (decimal)prediction.GoalsVisitor!)) points++;
    //    if (match.DoublePoints) points *= 2;
    //    return points;
    //}

    //public MatchStatus GetMatchStatus(int goalsLocal, int goalsVisitor)
    //{
    //    if (goalsLocal > goalsVisitor) return MatchStatus.LocalWin;
    //    if (goalsLocal < goalsVisitor) return MatchStatus.VisitorWin;
    //    return MatchStatus.Tie;
    //}
}