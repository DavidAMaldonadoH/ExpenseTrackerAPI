using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : Controller
{

}