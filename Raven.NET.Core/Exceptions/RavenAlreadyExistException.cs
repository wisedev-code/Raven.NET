namespace Raven.NET.Core.Exceptions
{
    public class RavenAlreadyExistException : RavenBaseException
    {
        public RavenAlreadyExistException(string ravenName) : base($"Raven with name: {ravenName} already exists")
        {
            Code = "RAVEN_ALREADY_REGISTERED";
        }
    }
}