using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using IntPtr = System.IntPtr;

namespace StlRust {

public sealed class StlLoader : MonoBehaviour
{
    [SerializeField] string _filename = "3DBenchy.stl";

    string FilePath => Application.streamingAssetsPath + "/" + _filename;

    unsafe void Start()
    {
        using var plugin = PluginHandle.Create(FilePath);

        // Vertex array construction
        var vertices = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>
          ((void*)plugin.VertexDataPointer, plugin.VertexCount, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        var guard = AtomicSafetyHandle.Create();
        NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref vertices, guard);
#endif

        // Index array construction
        using var indices = new NativeArray<int>
          (plugin.IndexCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        plugin.CopyIndexArray((IntPtr)indices.GetUnsafePtr(), (uint)indices.Length);

        // Mesh object construction
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
        mesh.RecalculateNormals();
        mesh.UploadMeshData(true);

        // Mesh replacing
        GetComponent<MeshFilter>().sharedMesh = mesh;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle.Release(guard);
#endif
    }
}

} // namespace StlRust
