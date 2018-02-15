using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.Common.Interfaces
{
    public interface IMessagingQueue
    {
        Task BeginProcessing(CancellationToken token);
        Task ResumeProcessing(CancellationToken token);
        Task PauseProcessing();
    }
}