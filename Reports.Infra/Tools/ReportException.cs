using System;

namespace Reports.Infra.Tools
{
    public class ReportException : Exception
    {
        public ReportException(string message)
            : base(message)
        {
        }
    }
}