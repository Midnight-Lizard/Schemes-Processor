using SemVer;

namespace MidnightLizard.Schemes.Infrastructure.Serialization.Common
{
    public interface IMessageMetadata
    {
        string Type { get; set; }
        Range VersionRange { get; set; }
    }
}