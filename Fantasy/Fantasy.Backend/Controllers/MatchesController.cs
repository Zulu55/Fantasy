using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class MatchesController : GenericController<Match>
{
    private readonly IMatchesUnitOfWork _matchesUnitOfWork;

    public MatchesController(IGenericUnitOfWork<Match> unitOfWork, IMatchesUnitOfWork matchesUnitOfWork) : base(unitOfWork)
    {
        _matchesUnitOfWork = matchesUnitOfWork;
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _matchesUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _matchesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _matchesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(MatchDTO matchDTO)
    {
        var action = await _matchesUnitOfWork.AddAsync(matchDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync(MatchDTO matchDTO)
    {
        var action = await _matchesUnitOfWork.UpdateAsync(matchDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}