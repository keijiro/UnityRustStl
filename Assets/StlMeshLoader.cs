using UnityEngine;
using StlRust;

public sealed class StlMeshLoader : MonoBehaviour
{
    [SerializeField] string _filename = "3DBenchy.stl";

    string FilePath => Application.streamingAssetsPath + "/" + _filename;

    void Start()
    {
        using var stl = StlMeshData.Open(FilePath);
        using var vertices = stl.GetVerticesAsNativeArray();
        using var indices = stl.GetIndicesAsNativeArray();

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
        mesh.RecalculateNormals();
        mesh.UploadMeshData(true);

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
