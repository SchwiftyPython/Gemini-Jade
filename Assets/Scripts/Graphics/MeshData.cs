using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Graphics
{
    [Flags]
    public enum MeshFlags 
    {
        Base = 1 << 0, // Vertices & triangles
        UV = 1 << 1,
        Color = 1 << 2,
        All = ~(~0 << 3)
    }
    
    /// <summary>
    /// Stores all the Mesh Data
    /// </summary>
    public class MeshData
    {
        public readonly List<Vector3> vertices;

        private readonly List<int> indices;

        public readonly List<Vector2> uvs;

        public readonly List<Color> colors;

        public Mesh mesh;

        private readonly MeshFlags _flags;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="flags"></param>
        public MeshData(MeshFlags flags = MeshFlags.Base)
        {
            vertices = new List<Vector3>();
            indices = new List<int>();
            colors = new List<Color>();
            uvs = new List<Vector2>();
            _flags = flags;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="planeCount"></param>
        /// <param name="flag"></param>
        /// <remarks>Most of our meshes are planes, so we know a plane
        /// is 4 vertices and 6 triangles, most of the time we will
        /// know the capacity of our lists.</remarks>
        public MeshData(int planeCount, MeshFlags flag = MeshFlags.Base)
        {
            vertices = new List<Vector3>(planeCount * 4);
            indices = new List<int>(planeCount * 6);
            colors = new List<Color>(
                (flag & MeshFlags.Color) == MeshFlags.Color ? planeCount * 4 : 0
            );
            uvs = new List<Vector2>(
                (flag & MeshFlags.UV) == MeshFlags.UV ? planeCount * 4 : 0
            );
            _flags = flag;
        }

        /// <summary>
        /// Add a triangle to our mesh
        /// </summary>
        /// <param name="vIndex"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void AddTriangle(int vIndex, int a, int b, int c)
        {
            indices.Add(vIndex + a);
            indices.Add(vIndex + b);
            indices.Add(vIndex + c);
        }

        /// <summary>
        /// Create a new mesh
        /// </summary>
        private void CreateNewMesh()
        {
            if (mesh != null)
            {
                Object.Destroy(mesh);
            }

            mesh = new Mesh();
        }

        /// <summary>
        /// Clear the MeshData
        /// </summary>
        public void Clear()
        {
            vertices.Clear();
            indices.Clear();
            colors.Clear();
            uvs.Clear();
        }

        /// <summary>
        /// Build our mesh
        /// </summary>
        /// <returns>A new mesh</returns>
        /// <seealso cref="Mesh"/>
        public Mesh Build()
        {
            CreateNewMesh();
            
            if (vertices.Count > 0)
            {
                if (indices.Count > 0)
                {
                    mesh.SetVertices(vertices);
                    mesh.SetTriangles(indices, 0);

                    if ((_flags & MeshFlags.UV) == MeshFlags.UV)
                    {
                        mesh.SetUVs(0, uvs);
                    }

                    if ((_flags & MeshFlags.Color) == MeshFlags.Color)
                    {
                        mesh.SetColors(colors);
                    }
                
                    mesh.RecalculateNormals();

                    return mesh;
                }
            }

            // todo Output some kind of error here?
            Object.Destroy(mesh);
            
            return null;
        }
    }
}