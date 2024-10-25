using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.Repositories.Interfaces;

public interface IGroupsRepository
{
    Task<ActionResponse<Group>> AddAsync(GroupDTO groupDTO);

    Task<ActionResponse<Group>> UpdateAsync(GroupDTO groupDTO);

    Task<ActionResponse<Group>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}