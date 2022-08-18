using System;

namespace Raven.NET.Core.Exceptions
{
    public class RavenBaseException : Exception
    {
        public RavenBaseException(string message)
        {
            _message = message;
        }

        public override string Message => _message + " Krraa!";
        protected string Code { get; set; }
        private string _message { get; }
    }
}