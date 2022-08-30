namespace Raven.NET.Core.Exceptions
{
    public class RavenAlreadyExistsException : RavenBaseException
    {
        public RavenAlreadyExistsException(string ravenName) : base($"Raven with name: {ravenName} already exists")
        {
            Code = "RAVEN_ALREADY_REGISTERED";
        }
    }
}