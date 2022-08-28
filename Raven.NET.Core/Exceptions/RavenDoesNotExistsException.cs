namespace Raven.NET.Core.Exceptions
{
    public class RavenDoesNotExistsException : RavenBaseException
    {
        public RavenDoesNotExistsException(string ravenName) : base($"Raven with provided name: {ravenName} does not exist")
        {
            Code = "RAVEN_NOT_FOUND";
        }
    }
}