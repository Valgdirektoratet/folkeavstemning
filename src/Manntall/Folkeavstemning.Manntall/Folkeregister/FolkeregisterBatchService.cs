using Manntall.Backend.Folkeregister.Integration;

namespace Manntall.Backend.Folkeregister;

public class FolkeregisterBatchService
{
    private readonly IFolkeregisterClient _folkeregisterClient;
    private readonly ILogger<FolkeregisterBatchService> _logger;

    public FolkeregisterBatchService(ILogger<FolkeregisterBatchService> logger, IFolkeregisterClient folkeregisterClient)
    {
        _logger = logger;
        _folkeregisterClient = folkeregisterClient;
    }

    public async Task<string[]> GetPersonerInUttrekk(string jobbId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetPersonerInUttrekk starting for job {JobbId}", jobbId);
        var batchNr = 0;
        var result = new List<string>();
        while (true)
        {
            var batch = await GetBatch(jobbId, batchNr, cancellationToken);
            switch (batch)
            {
                case BatchData batchData:
                    result.AddRange(batchData.Identifikasjonsnummer);
                    batchNr++;
                    break;
                case BatchError { ShouldRetry: true }:
                    await Task.Delay(2500, cancellationToken);
                    break;
                case BatchError error:
                    _logger.LogError("GetPersonerInUttrekk error {JobbId} {BatchNr} - failing with {Error}", jobbId, batchNr, error.Error);
                    throw new FolkeregisterException($"Could not get batch from Folkeregisteret: {error.Error}");
                default:
                    _logger.LogDebug("GetPersonerInUttrekk completed for job {JobbId}: got {Count} persons", jobbId, result.Count);
                    return result.ToArray();
            }
        }
    }

    private async Task<BatchResult> GetBatch(string jobbId, int batchNr, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetBatch for {JobbId} {BatchNr}", jobbId, batchNr);

        try
        {
            var response = await _folkeregisterClient.HentBatchAsync(jobbId, batchNr, cancellationToken);

            if (response.Feilmelding != null)
            {
                _logger.LogError("GetBatch for {JobbId} {BatchNr} received: {Feilmelding}", jobbId, batchNr, response.Feilmelding);
                return new BatchError(response.Feilmelding, false);
            }

            if (response.FoedselsEllerDNummer == null || response.FoedselsEllerDNummer.Count == 0)
            {
                return new NoMoreData();
            }

            return new BatchData(response.FoedselsEllerDNummer.ToArray());
        }
        catch (ApiException e) when(e.StatusCode == 404) // batch is not yet done
        {
            _logger.LogInformation("GetBatch for {JobbId} {BatchNr} got 404 - retrying", jobbId, batchNr);
            return new BatchError(e.Message, true);
        }
        catch (Exception e)
        {
            _logger.LogError("GetBatch for {JobbId} {BatchNr} failed: {Feilmelding}", jobbId, batchNr, e.Message);
            return new BatchError(e.Message, false);
        }
    }

    private record BatchResult;

    private record BatchData(string[] Identifikasjonsnummer) : BatchResult;

    private record NoMoreData : BatchResult;

    private record BatchError(string Error, bool ShouldRetry) : BatchResult;
}
