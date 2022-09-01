using System.Collections.Generic;
using GoRogue;
using UnityEngine;
using Utilities;

namespace Graphics
{
    public static class MeshPool
    {
        /// <summary>
        /// Dictionary of planes where the identifier is representative of the size of the plane.
        /// </summary>
        public static Dictionary<float, MeshData> planes = new Dictionary<float, MeshData>();

        public static Dictionary<int, MeshData> cornerPlanes = new Dictionary<int, MeshData>();

        public static Dictionary<float, MeshData> pawnPlanes = new Dictionary<float, MeshData>();

        /// <summary>
        /// Get the mesh for corners for a connected tile
        /// </summary>
        /// <param name="corners"></param>
        /// <returns></returns>
        public static Mesh GetCornersPlane(bool[] corners)
        {
            var id = HashUtils.HashBoolArray(corners);

            if (cornerPlanes.ContainsKey(id))
            {
                return cornerPlanes[id].mesh;
            }

            cornerPlanes.Add(id, GenCornersPlane(corners));

            return cornerPlanes[id].mesh;
        }

        /// <summary>
        /// Get a plane mesh of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <returns>Mesh of specified size</returns>
        public static Mesh GetPlaneMesh(Vector2 size)
        {
            var id = size.x + size.y * 666f;
            
            if (planes.ContainsKey(id))
            {
                return planes[id].mesh;
            }

            planes.Add(id, GenPlaneMesh(size));
            
            return planes[id].mesh;
        }

        /// <summary>
        /// Get a plane mesh of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns>Mesh of specified size</returns>
        public static Mesh GetPawnPlaneMesh(Vector2 size, Direction direction)
        {
            var id = size.x + size.y * 666f +  direction.GetHashCode() * 333f;
            
            if (pawnPlanes.ContainsKey(id))
            {
                return pawnPlanes[id].mesh;
            }

            pawnPlanes.Add(id, GenPawnMesh(size, direction));
            
            return pawnPlanes[id].mesh;
        }

        /// <summary>
        /// Generate the mesh for corners for a connected tile
        /// </summary>
        /// <param name="corners"></param>
        /// <returns></returns>
        public static MeshData GenCornersPlane(bool[] corners)
        {
            var meshData = new MeshData(4, MeshFlags.Base | MeshFlags.UV);
            for (var i = 0; i < 4; i++)
            {
                if (corners[i])
                {
                    var loc = Vector2.zero;
                    var sx = .42f;
                    var sy = .55f;

                    if (i == 1)
                    {
                        sy = .42f;
                        loc.y = 1 - sy;
                    }
                    else if (i == 2)
                    {
                        sy = .42f;
                        loc.x = 1 - sx;
                        loc.y = 1 - sy;
                    }
                    else if (i == 3)
                    {
                        loc.x = 1 - sx;
                    }

                    var vIndex = meshData.vertices.Count;
                    meshData.vertices.Add(new Vector3(loc.x, loc.y));
                    meshData.vertices.Add(new Vector3(loc.x, loc.y + sy));
                    meshData.vertices.Add(new Vector3(loc.x + sx, loc.y + sy));
                    meshData.vertices.Add(new Vector3(loc.x + sx, loc.y));
                    meshData.uvs.Add(new Vector2(0f, 0f));
                    meshData.uvs.Add(new Vector2(0f, 1f));
                    meshData.uvs.Add(new Vector2(1f, 1f));
                    meshData.uvs.Add(new Vector2(1f, 0f));
                    meshData.AddTriangle(vIndex, 0, 1, 2);
                    meshData.AddTriangle(vIndex, 0, 2, 3);
                }
            }

            meshData.Build();
            return meshData;
        }

        /// <summary>
        /// Generate a plane with uv corresponding to the direction of the character.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static MeshData GenPawnMesh(Vector2 size, Direction direction)
        {
            const float uy = 1f / 3f;

            var meshData = new MeshData(1, MeshFlags.Base | MeshFlags.UV);
            meshData.vertices.Add(new Vector3(0, 0));
            meshData.vertices.Add(new Vector3(0, size.y));
            meshData.vertices.Add(new Vector3(size.x, size.y));
            meshData.vertices.Add(new Vector3(size.x, 0));

            if (direction == Direction.UP)
            {
                meshData.uvs.Add(new Vector2(0f, uy));
                meshData.uvs.Add(new Vector2(0f, uy * 2f));
                meshData.uvs.Add(new Vector2(1f, uy * 2f));
                meshData.uvs.Add(new Vector2(1f, uy));
            }
            else if (direction == Direction.DOWN)
            {
                meshData.uvs.Add(new Vector2(0f, uy * 2f));
                meshData.uvs.Add(new Vector2(0f, 1f));
                meshData.uvs.Add(new Vector2(1f, 1f));
                meshData.uvs.Add(new Vector2(1f, uy * 2f));
            }
            else if (direction == Direction.RIGHT)
            {
                meshData.uvs.Add(new Vector2(0f, 0f));
                meshData.uvs.Add(new Vector2(0f, uy));
                meshData.uvs.Add(new Vector2(1f, uy));
                meshData.uvs.Add(new Vector2(1f, 0f));
            }
            else if (direction == Direction.LEFT)
            {
                meshData.uvs.Add(new Vector2(1f, 0f));
                meshData.uvs.Add(new Vector2(1f, uy));
                meshData.uvs.Add(new Vector2(0f, uy));
                meshData.uvs.Add(new Vector2(0f, 0f));
            }

            meshData.AddTriangle(0, 0, 1, 2);
            meshData.AddTriangle(0, 0, 2, 3);
            meshData.Build();
            return meshData;
        }

        /// <summary>
        /// Generate a plane mesh of the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static MeshData GenPlaneMesh(Vector2 size)
        {
            var meshData = new MeshData(1, MeshFlags.Base | MeshFlags.UV);
            meshData.vertices.Add(new Vector3(0, 0));
            meshData.vertices.Add(new Vector3(0, size.y));
            meshData.vertices.Add(new Vector3(size.x, size.y));
            meshData.vertices.Add(new Vector3(size.x, 0));
            meshData.uvs.Add(new Vector2(0f, 0f));
            meshData.uvs.Add(new Vector2(0f, 1f));
            meshData.uvs.Add(new Vector2(1f, 1f));
            meshData.uvs.Add(new Vector2(1f, 0f));
            meshData.AddTriangle(0, 0, 1, 2);
            meshData.AddTriangle(0, 0, 2, 3);
            meshData.Build();
            return meshData;
        }
    }
}