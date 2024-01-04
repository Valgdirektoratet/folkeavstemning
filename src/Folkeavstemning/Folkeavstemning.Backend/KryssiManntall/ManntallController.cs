using System.Text;
using Folkeavstemning.Api.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Configuration;
using Shared.Export;

namespace Folkeavstemning.Api.KryssiManntall;

[ApiController]
[EndpointGroupName("Manntall")]
[Route("kryss-i-manntall")]
[Authorize(Policy = "BasicAuthentication")]
public class ManntallController : ControllerBase
{
    private readonly FolkeavstemningContext _context;

    public ManntallController(FolkeavstemningContext context)
    {
        _context = context;
    }

    [HttpGet("export/{folkeavstemningId}")]
    public async Task<IActionResult> GetKryss(string folkeavstemningId, CancellationToken token)
    {
        if (!FolkeavstemningsKonfigurasjon.Exists(folkeavstemningId))
        {
            return NotFound();
        }

        var result = await _context.Stemmegivninger.Where(x => x.FolkeavstemningId == folkeavstemningId).Select(x => new {x.FolkeavstemningId, x.Manntallsnummer}).ToArrayAsync(token);

        var exportCsv = result.ExportCsv();

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(exportCsv)).ToArray();
        return File(bytes, "application/csv", $"Kryss i manntall - {folkeavstemningId}.csv");
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount() =>
        Ok(await _context.Stemmegivninger.GroupBy(x => x.FolkeavstemningId)
            .Select(x => new { FolkeavstemningId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.FolkeavstemningId, x => x.Count));
}
