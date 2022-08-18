namespace Raven.NET.Core.Exceptions
{
    public class RavenDoesNotExistException : RavenBaseException
    {
        public RavenDoesNotExistException(string ravenName) : base($"Raven with provided name: {ravenName} does not exist")
        {
            Code = "RAVEN_NOT_FOUND";
        }
    }
}