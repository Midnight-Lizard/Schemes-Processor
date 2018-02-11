using MidnightLizard.Schemes.Domain.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidnightLizard.Schemes.Domain.Common.Results
{
    public class MessageResult : DomainResult
    {
        public ITransportMessage<BaseMessage> Message { get; }

        public MessageResult(ITransportMessage<BaseMessage> message)
        {
            Message = message;
        }

        public MessageResult(string errorMessage) : base(errorMessage) { }

        public MessageResult(Exception ex) : base(ex) { }

        public MessageResult(bool hasError, Exception ex, string errorMessage)
            : base(hasError, ex, errorMessage) { }
    }
}
