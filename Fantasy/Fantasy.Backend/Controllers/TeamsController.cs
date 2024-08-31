using Fantasy.Backend.UnitsOfWork.Interfaces;
using Fantasy.Shared.Entites;
using Microsoft.AspNetCore.Mvc;

namespace Fantasy.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : GenericController<Team>
{
    public TeamsController(IGenericUnitOfWork<Team> unitOfWork) : base(unitOfWork)
    {
    }
}