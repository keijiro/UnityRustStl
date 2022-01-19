using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

public sealed class Test : MonoBehaviour
{
    #if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
    #else
    const string _dll = "stlrust";
    #endif

    [DllImport(_dll)]
    static extern IntPtr stlrust_open(string path);

    [DllImport(_dll)]
    static extern void stlrust_close(IntPtr ptr);

    [DllImport(_dll)]
    static extern uint stlrust_get_vertex_count(IntPtr ptr);

    [DllImport(_dll)]
    static extern uint stlrust_get_index_count(IntPtr ptr);

    [DllImport(_dll)]
    static extern IntPtr stlrust_get_vertex_pointer(IntPtr ptr);

    [DllImport(_dll)]
    static extern void
      stlrust_copy_index_array(IntPtr ptr, IntPtr buffer, uint size);

    unsafe void Start()
    {
        var stl = stlrust_open("Test.stl");

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        var handle = AtomicSafetyHandle.Create();
#endif
        var vcount = (int)stlrust_get_vertex_count(stl);
        var vdata = stlrust_get_vertex_pointer(stl);

        var vertices = NativeArrayUnsafeUtility
            .ConvertExistingDataToNativeArray<Vector3>
            ((void*)vdata, vcount, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        NativeArrayUnsafeUtility
            .SetAtomicSafetyHandle(ref vertices, handle);
#endif

        var icount = (int)stlrust_get_index_count(stl);
        var indices = new NativeArray<int>
          (icount, Allocator.Persistent,
           NativeArrayOptions.UninitializedMemory);

        stlrust_copy_index_array(stl, (IntPtr)indices.GetUnsafePtr(), (uint)icount);

        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
        mesh.RecalculateNormals();
        mesh.UploadMeshData(true);

        GetComponent<MeshFilter>().sharedMesh = mesh;

        indices.Dispose();
        stlrust_close(stl);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.Release(handle);
#endif

    }
}
