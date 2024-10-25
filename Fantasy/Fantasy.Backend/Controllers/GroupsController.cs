using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class GroupsController : GenericController<Group>
    {
        private readonly IGroupsUnitOfWork _groupsUnitOfWork;

        public GroupsController(IGenericUnitOfWork<Group> unitOfWork, IGroupsUnitOfWork groupsUnitOfWork) : base(unitOfWork)
        {
            _groupsUnitOfWork = groupsUnitOfWork;
        }

        [HttpGet("paginated")]
        public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
        {
            pagination.Email = User.Identity!.Name;
            var response = await _groupsUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalRecordsPaginated")]
        public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
        {
            pagination.Email = User.Identity!.Name;
            var action = await _groupsUnitOfWork.GetTotalRecordsAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var response = await _groupsUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }

        [HttpPost("full")]
        public async Task<IActionResult> PostAsync(GroupDTO groupDTO)
        {
            groupDTO.AdminId = User.Identity!.Name!;
            var action = await _groupsUnitOfWork.AddAsync(groupDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPut("full")]
        public async Task<IActionResult> PutAsync(GroupDTO groupDTO)
        {
            var action = await _groupsUnitOfWork.UpdateAsync(groupDTO);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }
    }
}