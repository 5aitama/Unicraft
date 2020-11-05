using Unity.Mathematics;
using Unity.Collections;

namespace Unicraft.Core
{
    public interface INoiseBehaviour
    {
        float Evaluate(in int3 localPosition, in int3 worldPosition, in int3 chunkSize);
    }

    public interface ICustomNoiseBehaviour<T> where T : struct, INoiseBehaviour
    {
        T GetNoiseBehaviour();
    }
}