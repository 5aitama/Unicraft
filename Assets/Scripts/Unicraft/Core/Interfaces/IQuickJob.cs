using Unity.Jobs;

namespace Unicraft.Core
{
    public interface IQuickJob
    {
        JobHandle QuickSchedule(in JobHandle deps = default);
    }
}