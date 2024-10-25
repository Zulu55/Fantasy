using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations;

public class UserGroupsUnitOfWork : GenericUnitOfWork<UserGroup>, IUserGroupsUnitOfWork

{
    private readonly IUserGroupsRepository _userGroupsRepository;

    public UserGroupsUnitOfWork(IGenericRepository<UserGroup> repository, IUserGroupsRepository userGroupsRepository) : base(repository)
    {
        _userGroupsRepository = userGroupsRepository;
    }

    public override async Task<ActionResponse<IEnumerable<UserGroup>>> GetAsync(PaginationDTO pagination) => await _userGroupsRepository.GetAsync(pagination);

    public override async Task<ActionResponse<UserGroup>> GetAsync(int id) => await _userGroupsRepository.GetAsync(id);

    public async Task<ActionResponse<UserGroup>> AddAsync(UserGroupDTO userGroupDTO) => await _userGroupsRepository.AddAsync(userGroupDTO);

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _userGroupsRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<UserGroup>> UpdateAsync(UserGroupDTO userGroupDTO) => await _userGroupsRepository.UpdateAsync(userGroupDTO);
}