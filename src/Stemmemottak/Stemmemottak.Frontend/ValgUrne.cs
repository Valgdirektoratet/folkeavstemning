using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Stemmemottak.Api;

[ApiController]
public class ValgUrne : ControllerBase
{
    private readonly IOptions<ResultatConfig> _config;

    public ValgUrne(IOptions<ResultatConfig> config)
    {
        _config = config;
    }

    /// <summary>
    /// Avlegg stemme. Stemmen må være signert
    /// </summary>
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [HttpPost("folkeavstemning/{folkeavstemningId}")]
    public async Task<IActionResult> LeggInnStemme(string folkeavstemningId, [FromBody]SignertStemmeDto dto)
    {
        if (!FolkeavstemningsKonfigurasjon.Exists(folkeavstemningId))
        {
            return NotFound();
        }

        try
        {
            var result =
                await _config.Value.Url
                    .AppendPathSegment("stemmemottak")
                    .AppendPathSegment(folkeavstemningId)
                    .AllowAnyHttpStatus()
                    .WithBasicAuth(_config.Value.Username, _config.Value.Password)
                    .PostJsonAsync(dto);

            var stringAsync = await result.GetStringAsync();
            return result.StatusCode switch
            {
                200 => Ok(stringAsync),
                400 => BadRequest(stringAsync),
                500 => Problem(stringAsync),
                _ => Ok(stringAsync)
            };
        }
        catch (Exception)
        {
            return Problem();
        }
    }
}
