namespace Raven.NET.Core.Exceptions
{
    public class RavenUpdateCallbackException : RavenBaseException
    {
        public RavenUpdateCallbackException(string ravenName, string exceptionMessage) : base($"Raven {ravenName} encountered error on performing update {exceptionMessage}")
        {
            Code = "RAVEN_NOT_FOUND";
        }
    }
}