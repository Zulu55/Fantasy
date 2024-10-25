using Fantasy.Backend.Repositories.Interfaces;
using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Fantasy.Shared.Responses;

namespace Fantasy.Backend.UnitsOfWork.Implementations
{
    public class GroupsUnitOfWork : GenericUnitOfWork<Group>, IGroupsUnitOfWork
    {
        private readonly IGroupsRepository _groupsRepository;

        public GroupsUnitOfWork(IGenericRepository<Group> repository, IGroupsRepository groupsRepository) : base(repository)
        {
            _groupsRepository = groupsRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination) => await _groupsRepository.GetAsync(pagination);

        public override async Task<ActionResponse<Group>> GetAsync(int id) => await _groupsRepository.GetAsync(id);

        public async Task<ActionResponse<Group>> AddAsync(GroupDTO groupDTO) => await _groupsRepository.AddAsync(groupDTO);

        public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _groupsRepository.GetTotalRecordsAsync(pagination);

        public async Task<ActionResponse<Group>> UpdateAsync(GroupDTO groupDTO) => await _groupsRepository.UpdateAsync(groupDTO);
    }
}