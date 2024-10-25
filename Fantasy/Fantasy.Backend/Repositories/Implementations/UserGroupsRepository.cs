using Fantasy.Backend.Data;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fantasy.Backend.Repositories.Implementations
{
    public class UserGroupsRepository : GenericRepository<UserGroup>, IUserGroupsRepository

    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public UserGroupsRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context = context;
            _usersRepository = usersRepository;
        }

        public override async Task<ActionResponse<IEnumerable<UserGroup>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.UserGroups
                .Include(x => x.User)
                .AsQueryable();
            queryable = queryable.Where(x => x.GroupId == pagination.Id);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                 x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<UserGroup>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.User.FirstName)
                    .ThenBy(x => x.User.LastName)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<UserGroup>> GetAsync(int id)
        {
            var userGroup = await _context.UserGroups
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (userGroup == null)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR001"
                };
            }

            return new ActionResponse<UserGroup>
            {
                WasSuccess = true,
                Result = userGroup
            };
        }

        public async Task<ActionResponse<UserGroup>> AddAsync(UserGroupDTO userGroupDTO)
        {
            var user = await _usersRepository.GetUserAsync(Guid.Parse(userGroupDTO.UserId));
            if (user == null)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR013"
                };
            }

            var group = await _context.Groups.FindAsync(userGroupDTO.GroupId);
            if (group == null)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR014"
                };
            }

            var userGroup = new UserGroup
            {
                Group = group,
                User = user
            };

            _context.Add(userGroup);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = true,
                    Result = userGroup
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
        {
            var queryable = _context.UserGroups.AsQueryable();
            queryable = queryable.Where(x => x.GroupId == pagination.Id);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.User.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||
                                                 x.User.LastName.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = (int)count
            };
        }

        public async Task<ActionResponse<UserGroup>> UpdateAsync(UserGroupDTO userGroupDTO)
        {
            var currentUserGroup = await _context.UserGroups.FindAsync(userGroupDTO.Id);
            if (currentUserGroup == null)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR015"
                };
            }

            currentUserGroup.IsActive = userGroupDTO.IsActive;

            _context.Update(currentUserGroup);
            try
            {
                await _context.SaveChangesAsync();
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = true,
                    Result = currentUserGroup
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = "ERR003"
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<UserGroup>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
    }
}