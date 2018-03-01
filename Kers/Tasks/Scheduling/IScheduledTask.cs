using System.Threading;
using System.Threading.Tasks;

namespace Kers.Tasks
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}