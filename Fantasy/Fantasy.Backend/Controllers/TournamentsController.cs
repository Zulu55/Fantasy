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
public class TournamentsController : GenericController<Tournament>
{
    private readonly ITournamentsUnitOfWork _tournamentsUnitOfWork;

    public TournamentsController(IGenericUnitOfWork<Tournament> unitOfWork, ITournamentsUnitOfWork tournamentsUnitOfWork) : base(unitOfWork)
    {
        _tournamentsUnitOfWork = tournamentsUnitOfWork;
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _tournamentsUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var response = await _tournamentsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("totalRecordsPaginated")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _tournamentsUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest();
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _tournamentsUnitOfWork.GetComboAsync());
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(TournamentDTO tournamentDTO)
    {
        var action = await _tournamentsUnitOfWork.AddAsync(tournamentDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutAsync(TournamentDTO tournamentDTO)
    {
        var action = await _tournamentsUnitOfWork.UpdateAsync(tournamentDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}