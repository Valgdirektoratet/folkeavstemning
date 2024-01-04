using Microsoft.AspNetCore.Mvc;
using Shared.Configuration;

namespace Folkeavstemning.Api.Folkeavstemning;

[Route("api/folkeavstemning")]
[ApiController]
[EndpointGroupName("Folkeavstemning")]
public class FolkeavstemningController : ControllerBase
{
    private static readonly Lazy<FolkeavstemningDto[]> Folkeavstemninger = new(() => FolkeavstemningsKonfigurasjon.Folkeavstemninger.Select(f => f.MapToFolkeavstemningToDto()).ToArray());

    [HttpGet]
    public ActionResult<FolkeavstemningDto[]> GetFolkeavstemningAsync() => Folkeavstemninger.Value;
}
