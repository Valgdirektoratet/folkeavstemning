namespace Manntall.Backend.Folkeregister;

public class FolkeregisterException : Exception
{
    public FolkeregisterException(string responseFeilmelding) : base(responseFeilmelding)
    {
    }
}