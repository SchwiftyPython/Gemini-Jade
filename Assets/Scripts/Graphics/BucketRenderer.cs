using System.Collections.Generic;
using UnityEngine;
using World;

namespace Graphics
{
    public class BucketRenderer
    {
        private bool _redraw = true;

        private Vector3 _position;
        
        public LayerGridBucket Bucket { get; protected set; }
        
        public MapLayer Layer { get; protected set; }
        
        public Dictionary<int, MeshData> Meshes { get; protected set; }

        public BucketRenderer(LayerGridBucket bucket, MapLayer layer)
        {
            Bucket = bucket;

            Layer = layer;

            Meshes = new Dictionary<int, MeshData>();

            _position = new Vector3(0, 0);
        }

        public MeshData GetMeshData(int graphicInstance, bool useSize = true, MeshFlags flag = MeshFlags.Base)
        {
            if (Meshes.ContainsKey(graphicInstance))
            {
                return Meshes[graphicInstance];
            }

            Meshes.Add(graphicInstance, useSize ? new MeshData(Bucket.Rect.Area, flag) : new MeshData(flag));

            return Meshes[graphicInstance];
        }

        public void Draw()
        {
            if (_redraw)
            {
                BuildMeshes();

                _redraw = false;
            }

            foreach (var meshData in Meshes)
            {
                UnityEngine.Graphics.DrawMesh(meshData.Value.mesh, _position, Quaternion.identity,
                    GraphicInstance.instances[meshData.Key].Material, 0);
            }
        }

        public void ClearMeshes()
        {
            foreach (var meshData in Meshes.Values)
            {
                meshData.Clear();
            }
        }

        public virtual void BuildMeshes()
        {
            foreach (var tile in Bucket.Tiles)
            {
                if (tile == null || tile.hidden)
                {
                    continue;
                }

                var currentMeshData = GetMeshData(tile.MainGraphic.Uid);

                var vertexIndex = currentMeshData.vertices.Count;
                
                currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y+1));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X+1, tile.Position.Y+1));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X+1, tile.Position.Y));

                currentMeshData.AddTriangle(vertexIndex, 0, 1, 2);
                currentMeshData.AddTriangle(vertexIndex, 0, 2, 3);
            }

            foreach (var meshData in Meshes.Values)
            {
                meshData.Build();
            }
        }
    }
}
