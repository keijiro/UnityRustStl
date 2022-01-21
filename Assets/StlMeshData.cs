using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace StlRust {

public sealed class StlMeshData : SafeHandleZeroOrMinusOneIsInvalid
{
    #region SafeHandle implementation

    StlMeshData() : base(true) {}

    protected override bool ReleaseHandle()
    {
        stlrust_close(handle);
        return true;
    }

    #endregion

    #region Public methods

    public static StlMeshData Open(string path) => stlrust_open(path);

    public int VertexCount => (int)stlrust_get_vertex_count(this);
    public int IndexCount => (int)stlrust_get_index_count(this);

    public IntPtr VertexDataPointer
      => stlrust_get_vertex_pointer(this);

    public void CopyIndexArray(IntPtr ptr, uint size)
      => stlrust_copy_index_array(this, ptr, size);

    #endregion

    #region Unmanaged interface

    #if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
    #else
    const string _dll = "stlrust";
    #endif

    [DllImport(_dll)] static extern
      StlMeshData stlrust_open(string path);

    [DllImport(_dll)] static extern
      void stlrust_close(IntPtr ptr);

    [DllImport(_dll)] static extern
      uint stlrust_get_vertex_count(StlMeshData self);

    [DllImport(_dll)] static extern
      uint stlrust_get_index_count(StlMeshData self);

    [DllImport(_dll)] static extern
      IntPtr stlrust_get_vertex_pointer(StlMeshData self);

    [DllImport(_dll)] static extern
      void stlrust_copy_index_array(StlMeshData self, IntPtr buffer, uint size);

    #endregion
}

} // namespace StlRust
