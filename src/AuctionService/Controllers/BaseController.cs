using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[Route("api/[controller]")]
[ApiController]
// [Authorize]
public abstract class BaseController : ControllerBase
{
}
    