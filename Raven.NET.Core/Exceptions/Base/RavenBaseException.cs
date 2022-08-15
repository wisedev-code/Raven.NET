using System;

namespace Raven.NET.Core.Exceptions
{
    public class RavenBaseException : Exception
    {
        public RavenBaseException(string code)
        {
            Code = code;
        }
        
        internal string Code { get; }
    }
}