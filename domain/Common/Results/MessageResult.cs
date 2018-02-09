using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Results
{
    public class MessageResult : DomainResult
    {
        public BaseMessage BaseMessage { get; }

        public MessageResult() { }

        public MessageResult(BaseMessage baseMessage)
        {
            BaseMessage = baseMessage;
        }

        public MessageResult(string errorMessage) : base(errorMessage) { }

        public MessageResult(Exception ex) : base(ex) { }

        public MessageResult(bool hasError, Exception ex, string errorMessage)
            : base(hasError, ex, errorMessage) { }
    }
}
