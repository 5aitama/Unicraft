using Unity.Jobs;

namespace MinecraftLike
{
    public interface IQuickJob
    {
        JobHandle QuickSchedule(in JobHandle deps = default);
    }
}