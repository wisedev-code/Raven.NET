using System;

namespace Raven.NET.Core.Exceptions
{
    public class RavenForTypeAlreadyExistsException : RavenBaseException
    {
        public RavenForTypeAlreadyExistsException(Type type) : base($"Raven for type {type} is already registered")
        {
            Code = "RAVEN_ALREADY_REGISTERED";
        }
    }
}