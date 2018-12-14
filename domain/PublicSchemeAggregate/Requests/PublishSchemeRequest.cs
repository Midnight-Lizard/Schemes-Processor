namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Requests
{
    public class PublishSchemeRequest : SchemeDomainRequest
    {
        public virtual string Description { get; private set; }
        public virtual ColorScheme ColorScheme { get; private set; }
    }
}
