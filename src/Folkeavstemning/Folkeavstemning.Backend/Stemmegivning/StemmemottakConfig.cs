using System.ComponentModel.DataAnnotations;

namespace Folkeavstemning.Api.Stemmegivning;

public class StemmemottakConfig
{
    [Required]
    public required string Url { get; set; }
}
