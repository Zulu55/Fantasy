using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : GenericController<Team>
{
    private readonly ITeamsUnitOfWork _teamsUnitOfWork;

    public TeamsController(IGenericUnitOfWork<Team> unitOfWork, ITeamsUnitOfWork teamsUnitOfWork) : base(unitOfWork)
    {
        _teamsUnitOfWork = teamsUnitOfWork;
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var response = await _teamsUnitOfWork.GetAsync();
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _teamsUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _teamsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _teamsUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("combo/{countryId:int}")]
    public async Task<IActionResult> GetComboAsync(int countryId)
    {
        return Ok(await _teamsUnitOfWork.GetComboAsync(countryId));
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(TeamDTO teamDTO)
    {
        var action = await _teamsUnitOfWork.AddAsync(teamDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync(TeamDTO teamDTO)
    {
        var action = await _teamsUnitOfWork.UpdateAsync(teamDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}