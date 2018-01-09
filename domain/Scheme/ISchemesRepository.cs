namespace MidnightLizard.Schemes.Domain.Scheme
{
    public interface ISchemesRepository
    {
        void Save(SchemeAggregateRoot scheme);
    }
}