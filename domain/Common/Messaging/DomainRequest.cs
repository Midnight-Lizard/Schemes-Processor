using MediatR;
using MidnightLizard.Schemes.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MidnightLizard.Schemes.Domain.Common.Messaging
{
    public abstract class DomainRequest<TAggregateId> : DomainMessage<TAggregateId>
        where TAggregateId : DomainEntityId
    {
    }
}
