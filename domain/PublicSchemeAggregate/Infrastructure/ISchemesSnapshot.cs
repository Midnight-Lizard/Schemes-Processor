using System;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Infrastructure
{
    public interface ISchemesSnapshot
    {
        void Save(PublicScheme scheme);

        PublicScheme Read(Guid id);
    }
}