namespace Folkeavstemning.Bff.Driftsmeldinger;

public class Driftsmeldinger : Dictionary<string, Driftsmelding>
{

}
public class Driftsmelding
{
    public DriftmeldingsType Type { get; set; } = DriftmeldingsType.Information;
    public string Tittel { get; set; } = string.Empty;
    public string Informasjon { get; set; } = string.Empty;
}

public enum DriftmeldingsType
{
    Success,
    Information,
    Error,
    Warning
}
