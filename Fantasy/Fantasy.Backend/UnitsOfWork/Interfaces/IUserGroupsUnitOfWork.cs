using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Interfaces;

public interface IUserGroupsUnitOfWork
{
    Task<ActionResponse<UserGroup>> AddAsync(UserGroupDTO userGroupDTO);

    Task<ActionResponse<UserGroup>> UpdateAsync(UserGroupDTO userGroupDTO);

    Task<ActionResponse<UserGroup>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<UserGroup>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}