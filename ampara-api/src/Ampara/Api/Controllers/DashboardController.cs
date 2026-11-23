using Ampara.Application.Common;
using Ampara.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = Roles.Staff)]
public class DashboardController(DashboardService dashboard) : ControllerBase
{
    [HttpGet]
    public Task<DashboardResponse> Get(CancellationToken cancellationToken) => dashboard.GetAsync(cancellationToken);
}
