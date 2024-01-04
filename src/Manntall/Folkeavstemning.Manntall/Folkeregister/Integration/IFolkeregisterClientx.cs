namespace Manntall.Backend.Folkeregister.Integration;

public interface IFolkeregisterClient
{
    /// <param name="body"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Henter persondata for et sett med personer.
    /// </summary>
    /// <param name="part">Hent ut et utsnitt av personen. Spesifiser hvilke entiteter som skal hentes ut. F.eks. identifikasjonsnummer og identitetsgrunnlag.</param>
    /// <param name="somBestilt">Henter personer i den rekkefølge de kommer i requestet. Evt. duplikate identifikatorer vil gi duplikate treff. For utgåtte identifikatorer eller identifikatorer som ikke finnes vil man få feilmelding i responsen istedet for persondata.</param>
    /// <returns>Returnerer en liste av oppslag.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<PersonBulkoppslagResponse> HentPersonerAsync(IEnumerable<string>? part = null, bool? somBestilt = null, PersonBulkoppslagRequest? body = null, CancellationToken cancellationToken = default(CancellationToken));

    /// <param name="body"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Lager en async tilpasset uttrekksjobb
    /// </summary>
    /// <returns>Returnerer en jobb id.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<UttrekkJobbResponse> LagJobbAsync(TilpassetUttrekkJobbRequest? body = null, CancellationToken cancellationToken = default(CancellationToken));

    /// <param name="batchnr"></param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <param name="jobbid"></param>
    /// <summary>
    /// Henter identifikatorer for angitt jobb
    /// </summary>
    /// <returns>Returnerer liste med forespurte identifikatorer.</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<UttrekkDataResponse> HentBatchAsync(string jobbid, long batchnr, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Henter siste sekvensnummer
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    Task<long> SisteSekvensnummerIFeedAsync(CancellationToken cancellationToken = default(System.Threading.CancellationToken));
}
