using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations
{
    public class PredictionsRepository : GenericRepository<Prediction>, IPredictionsRepository

    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public PredictionsRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context = context;
            _usersRepository = usersRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Prediction>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Predictions
                .Include(x => x.Match)
                .ThenInclude(x => x.Local)
                .Include(x => x.Match)
                .ThenInclude(x => x.Visitor)
                .AsQueryable();
            queryable = queryable.Where(x => x.GroupId == pagination.Id);
            queryable = queryable.Where(x => x.User.Email == pagination.Email);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Match.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                 x.Match.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Prediction>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Match.IsClosed)
                    .ThenBy(x => x.Match.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<Prediction>> GetAsync(int id)
        {
            var prediction = await _context.Predictions
                .Include(x => x.Match)
                .ThenInclude(x => x.Local)
                .Include(x => x.Match)
                .ThenInclude(x => x.Visitor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (prediction == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<Prediction>
            {
                WasSuccess = true,
                Result = prediction
            };
        }

        public async Task<ActionResponse<Prediction>> AddAsync(PredictionDTO predictionDTO)
        {
            var user = await _usersRepository.GetUserAsync(Guid.Parse(predictionDTO.UserId));
            if (user == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR013"
                };
            }

            var group = await _context.Groups.FindAsync(predictionDTO.GroupId);
            if (group == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR014"
                };
            }

            var tournament = await _context.Tournaments.FindAsync(predictionDTO.TournamentId);
            if (tournament == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR009"
                };
            }

            var match = await _context.Matches.FindAsync(predictionDTO.MatchId);
            if (match == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR012"
                };
            }

            var prediction = new Prediction
            {
                GoalsLocal = predictionDTO.GoalsLocal,
                GoalsVisitor = predictionDTO.GoalsVisitor,
                Group = group,
                Tournament = tournament,
                Match = match,
                User = user,
            };

            _context.Add(prediction);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Prediction>
                {
                    WasSuccess = true,
                    Result = prediction
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            var queryable = _context.Predictions.AsQueryable();
            queryable = queryable.Where(x => x.GroupId == pagination.Id);
            queryable = queryable.Where(x => x.User.Email == pagination.Email);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Match.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                 x.Match.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<Prediction>> UpdateAsync(PredictionDTO predictionDTO)
        {
            var currentPrediction = await _context.Predictions
                .Include(x => x.Match)
                .FirstOrDefaultAsync(x => x.Id == predictionDTO.Id);
            if (currentPrediction == null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR016"
                };
            }

            if (currentPrediction.Match.GoalsLocal != null || currentPrediction.Match.GoalsVisitor != null)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR018"
                };
            }

            if (CanWatch(currentPrediction))
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR018"
                };
            }

            currentPrediction.GoalsLocal = predictionDTO.GoalsLocal;
            currentPrediction.GoalsVisitor = predictionDTO.GoalsVisitor;
            currentPrediction.Points = predictionDTO.Points;

            _context.Update(currentPrediction);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<Prediction>
                {
                    WasSuccess = true,
                    Result = currentPrediction
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Prediction>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        //public async Task<ActionResponse<IEnumerable<PositionDTO>>> GetPositionsAsync(PaginationDTO pagination)
        //{
        //    var queryable = _context.Predictions
        //        .Where(x => x.GroupId == pagination.Id && x.Points.HasValue)
        //        .GroupBy(x => x.User)
        //        .Select(g => new PositionDTO
        //        {
        //            User = g.Key,
        //            Points = g.Sum(x => x.Points ?? 0)
        //        })
        //        .OrderByDescending(x => x.Points)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //    {
        //        queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
        //                                            x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        //    }

        //    return new ActionResponse<IEnumerable<PositionDTO>>
        //    {
        //        WasSuccess = true,
        //        Result = await queryable
        //            .Paginate(pagination)
        //            .ToListAsync()
        //    };
        //}

        //public async Task<ActionResponse<int>> GetTotalRecordsForPositionsAsync(PaginationDTO pagination)
        //{
        //    var queryable = _context.Predictions
        //        .Where(x => x.GroupId == pagination.Id && x.Points.HasValue)
        //        .GroupBy(x => x.User)
        //        .Select(g => new PositionDTO
        //        {
        //            User = g.Key,
        //            Points = g.Sum(x => x.Points ?? 0)
        //        })
        //        .OrderByDescending(x => x.Points)
        //        .AsQueryable();

        //    if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //    {
        //        queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
        //                                            x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        //    }

        //    double count = await queryable.CountAsync();
        //    return new ActionResponse<int>
        //    {
        //        WasSuccess = true,
        //        Result = (int)count
        //    };
        //}

        //public async Task<ActionResponse<IEnumerable<Prediction>>> GetAllPredictionsAsync(PaginationDTO pagination)
        //{
        //    var queryable = _context.Predictions
        //        .Include(x => x.Match)
        //        .ThenInclude(x => x.Local)
        //        .Include(x => x.Match)
        //        .ThenInclude(x => x.Visitor)
        //        .Include(x => x.User)
        //        .AsQueryable();
        //    queryable = queryable.Where(x => x.GroupId == pagination.Id);
        //    queryable = queryable.Where(x => x.MatchId == pagination.Id2);

        //    if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //    {
        //        queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
        //                                            x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        //    }

        //    return new ActionResponse<IEnumerable<Prediction>>
        //    {
        //        WasSuccess = true,
        //        Result = await queryable
        //            .OrderBy(x => x.User.FirstName)
        //            .ThenBy(x => x.User.LastName)
        //            .Paginate(pagination)
        //            .ToListAsync()
        //    };
        //}

        //public async Task<ActionResponse<int>> GetTotalRecordsAllPredictionsAsync(PaginationDTO pagination)
        //{
        //    var queryable = _context.Predictions.AsQueryable();
        //    queryable = queryable.Where(x => x.GroupId == pagination.Id);
        //    queryable = queryable.Where(x => x.MatchId == pagination.Id2);

        //    if (!string.IsNullOrWhiteSpace(pagination.Filter))
        //    {
        //        queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
        //                                            x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        //    }

        //    double count = await queryable.CountAsync();
        //    return new ActionResponse<int>
        //    {
        //        WasSuccess = true,
        //        Result = (int)count
        //    };
        //}

        public async Task<ActionResponse<IEnumerable<Prediction>>> GetBalanceAsync(PaginationDTO pagination)
        {
            var queryable = _context.Predictions
                .Include(x => x.Match)
                .ThenInclude(x => x.Local)
                .Include(x => x.Match)
                .ThenInclude(x => x.Visitor)
                .Include(x => x.User)
                .AsQueryable();
            queryable = queryable.Where(x => x.Match.GoalsLocal != null && x.Match.GoalsVisitor != null);
            queryable = queryable.Where(x => x.GroupId == pagination.Id);
            queryable = queryable.Where(x => x.User.Email == pagination.Email);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Match.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                    x.Match.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<Prediction>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.User.FirstName)
                    .ThenBy(x => x.User.LastName)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<ActionResponse<int>> GetTotalRecordsBalanceAsync(PaginationDTO pagination)
        {
            var queryable = _context.Predictions.AsQueryable();
            queryable = queryable.Where(x => x.Match.GoalsLocal != null && x.Match.GoalsVisitor != null);
            queryable = queryable.Where(x => x.GroupId == pagination.Id);
            queryable = queryable.Where(x => x.User.Email == pagination.Email);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Match.Local.Name.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                 x.Match.Visitor.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public virtual bool CanWatch(Prediction prediction)
        {
            if (prediction.Match.GoalsLocal != null || prediction.Match.GoalsVisitor != null)
            {
                return true;
            }

            var dateMatch = prediction.Match.Date.ToLocalTime();
            var currentDate = DateTime.Now;
            var minutesMatch = dateMatch.Subtract(DateTime.MinValue).TotalMinutes;
            var minutesNow = currentDate.Subtract(DateTime.MinValue).TotalMinutes;
            var difference = minutesNow - minutesMatch;
            var canWatch = difference >= -10;
            return canWatch;
        }
    }
}