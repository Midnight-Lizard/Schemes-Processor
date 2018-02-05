using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common
{
    public class DomainResult
    {
        public bool HasError { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Exception Exception { get; protected set; }

        public DomainResult() { }

        public DomainResult(string errorMessage)
        {
            HasError = true;
            ErrorMessage = errorMessage;
        }

        public DomainResult(Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
            Exception = ex;
        }

        public DomainResult(bool hasError, Exception ex, string errorMessage)
        {
            HasError = hasError;
            Exception = ex;
            ErrorMessage = errorMessage;
        }

        public static DomainResult Ok = new DomainResult
        {
            HasError = false
        };

        public static DomainResult Error = new DomainResult
        {
            HasError = true,
            ErrorMessage = "Internal server error"
        };
    }
}
