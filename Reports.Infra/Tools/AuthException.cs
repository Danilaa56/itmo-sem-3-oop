using System;

namespace Reports.Infra.Tools
{
    public class AuthException : Exception
    {
        public AuthException(string message)
            : base(message)
        {
        }
    }
}