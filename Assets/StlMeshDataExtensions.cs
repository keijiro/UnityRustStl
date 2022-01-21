using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Vector3 = UnityEngine.Vector3;
using IntPtr = System.IntPtr;

namespace StlRust {

public static class StlMeshDataNativeArrayExtension
{
#if ENABLE_UNITY_COLLECTIONS_CHECKS
    static AtomicSafetyHandle _guard;
    static bool _initialized;
#endif

    public unsafe static NativeArray<Vector3>
      GetVerticesAsNativeArray(this StlMeshData self)
    {
        var array =
          NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>
            ((void*)self.VertexDataPointer, self.VertexCount, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        if (!_initialized)
        {
            _guard = AtomicSafetyHandle.Create();
            _initialized = true;
        }

        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, _guard);
#endif

        return array;
    }

    public unsafe static NativeArray<int>
      GetIndicesAsNativeArray(this StlMeshData self)
    {
        var array =
          new NativeArray<int>
            (self.IndexCount, Allocator.Persistent,
             NativeArrayOptions.UninitializedMemory);

        self.CopyIndexArray((IntPtr)array.GetUnsafePtr(), (uint)array.Length);

        return array;
    }
}

} // namespace StlRust
