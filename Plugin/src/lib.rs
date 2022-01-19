use std::ffi::CStr;
use std::os::raw::c_char;
use std::fs::OpenOptions;

extern crate stl_io;
use stl_io::{Vertex, IndexedTriangle, IndexedMesh};

#[no_mangle]
pub unsafe extern "C" fn stlrust_open(cpath: *const c_char) -> *mut IndexedMesh {
    let path = CStr::from_ptr(cpath).to_str().unwrap();
    let mut file = OpenOptions::new().read(true).open(path).unwrap();
    let mut stl = stl_io::create_stl_reader(&mut file).unwrap();
    let mesh = stl.as_indexed_triangles().unwrap();
    Box::into_raw(Box::new(mesh))
}

#[no_mangle]
pub unsafe extern "C" fn stlrust_close(mesh: *mut IndexedMesh) {
    Box::from_raw(mesh);
}

#[no_mangle]
pub unsafe extern "C" fn stlrust_get_vertex_count(mesh: *const IndexedMesh) -> u32 {
    (*mesh).vertices.len() as u32
}

#[no_mangle]
pub unsafe extern "C" fn stlrust_get_index_count(mesh: *const IndexedMesh) -> u32 {
    (*mesh).faces.len() as u32 * 3
}

#[no_mangle]
pub unsafe extern "C" fn stlrust_get_vertex_pointer(mesh: *const IndexedMesh) -> *const Vertex {
    (*mesh).vertices.as_ptr()
}

#[no_mangle]
pub unsafe extern "C" fn stlrust_copy_index_array(mesh: *const IndexedMesh, buffer: *mut u32, size: u32) {
    let src = &(*mesh).faces;
    let buffer = std::slice::from_raw_parts_mut(buffer, size as usize);
    let mut offs = 0;
    for f in src {
        buffer[offs + 0] = f.vertices[0] as u32;
        buffer[offs + 1] = f.vertices[1] as u32;
        buffer[offs + 2] = f.vertices[2] as u32;
        offs += 3;
    }
}
