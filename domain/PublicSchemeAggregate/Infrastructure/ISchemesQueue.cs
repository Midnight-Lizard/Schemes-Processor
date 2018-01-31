using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidnightLizard.Schemes.Domain.PublicSchemeAggregate.Infrastructure
{
    public interface ISchemesQueue
    {
        /// <summary>
        /// Start reading messages from queue
        /// </summary>
        /// <remarks>
        /// Starts only if it is not paused otherwise call is ignored
        /// If paused use <see cref="Resume(CancellationToken)"/> method instead
        /// </remarks>
        /// <param name="token">IApplicationLifetime.ApplicationStopping CancellationToken</param>
        /// <returns>When app is stopping or when queue is paused</returns>
        Task BeginProcessing(CancellationToken token);

        /// <summary>
        /// Pauses reading messages from queue and waits for the completion
        /// </summary>
        Task Pause();

        /// <summary>
        /// Resume reading messages from queue after it has been paused or starts if it was stopped
        /// </summary>
        /// <param name="token">IApplicationLifetime.ApplicationStopping CancellationToken</param>
        /// <returns>When app is stopping or when queue is paused</returns>
        Task Resume(CancellationToken token);
    }
}
