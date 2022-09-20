using System.Collections.Generic;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using World;

namespace Graphics
{
    /// <summary>
    /// Renders and blends ground textures
    /// </summary>
    public class BucketGroundRenderer : BucketRenderer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="layer"></param>
        public BucketGroundRenderer(LayerGridBucket bucket, MapLayer layer) : base(bucket, layer)
        {
        }

        /// <summary>
        /// Builds meshes
        /// </summary>
        public override void BuildMeshes()
        {
            var neighborGraphicIdList = new List<int>();

            var neighborGraphicIdsArr = new int[8];

            foreach (var baseObject in Bucket.BaseObjects)
            {
                if (baseObject.hidden)
                {
                    continue;
                }
                
                neighborGraphicIdList.Clear();
                
                var currentMeshData = GetMeshData(baseObject.MainGraphic.Uid, false, MeshFlags.Base | MeshFlags.Color);

                InitializeMesh(currentMeshData, baseObject);

                BuildNeighborLists((Tile) baseObject, neighborGraphicIdList, neighborGraphicIdsArr);

                BlendNeighbors(neighborGraphicIdList, neighborGraphicIdsArr, baseObject);
            }

            foreach (var meshData in Meshes.Values)
            {
                meshData.Build();
            }
        }

        /// <summary>
        /// Builds neighbor lists
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="neighborGraphicIdList"></param>
        /// <param name="neighborGraphicIdsArr"></param>
        private static void BuildNeighborLists(Tile tile, ICollection<int> neighborGraphicIdList,
            IList<int> neighborGraphicIdsArr)
        {
            foreach (var directionType in CollectionUtils.EnumToArray<Direction.Types>())
            {
                if (directionType == Direction.Types.NONE)
                {
                    continue;
                }

                var neighbor = tile.GetAdjacentTileByDirection(Direction.ToDirection(directionType));

                if (neighbor != null)
                {
                    neighborGraphicIdsArr[(int) directionType] = neighbor.MainGraphic.Uid;

                    if (neighborGraphicIdList.Contains(neighbor.MainGraphic.Uid))
                    {
                        continue;
                    }

                    if (neighbor.MainGraphic.Uid == tile.MainGraphic.Uid)
                    {
                        continue;
                    }

                    if (neighbor.GetMaxHeight() >= tile.GetMaxHeight())
                    {
                        neighborGraphicIdList.Add(neighbor.MainGraphic.Uid);
                    }
                }
                else
                {
                    neighborGraphicIdsArr[(int) directionType] = tile.MainGraphic.Uid;
                }
            }
        }

        /// <summary>
        /// Builds vertices for neighbor mesh in all directions.
        /// Starts from North and goes clockwise.
        /// </summary>
        /// <param name="currentMeshData"></param>
        /// <param name="tile"></param>
        /// <param name="z"></param>
        /// <seealso cref="GoRogue.Direction"/>
        private static void BuildNeighborVertices(MeshData currentMeshData, IGameObject tile, float z)
        {
            currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y + .5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y + .5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y - .5f, z)); 
            currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y - .5f, z));         
            currentMeshData.vertices.Add(new Vector3(tile.Position.X - .5f, tile.Position.Y - .5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X - .5f, tile.Position.Y, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X - .5f, tile.Position.Y + .5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y, z));
        }

        /// <summary>
        /// Sets up a basic mesh
        /// </summary>
        /// <param name="currentMeshData"></param>
        /// <param name="tile"></param>
        private static void InitializeMesh(MeshData currentMeshData, BaseObject tile)
        {
            var vIndex = currentMeshData.vertices.Count;
                
            var z = tile.MainGraphic.Priority;
            
            currentMeshData.vertices.Add(new Vector3(tile.Position.X - .5f, tile.Position.Y - .5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X - .5f, tile.Position.Y+.5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X+.5f, tile.Position.Y+.5f, z));
            currentMeshData.vertices.Add(new Vector3(tile.Position.X+.5f, tile.Position.Y- .5f, z));
                
            currentMeshData.colors.Add(Color.white);
            currentMeshData.colors.Add(Color.white);
            currentMeshData.colors.Add(Color.white);
            currentMeshData.colors.Add(Color.white);
                
            currentMeshData.AddTriangle(vIndex, 0, 1, 2);
            currentMeshData.AddTriangle(vIndex, 0, 2, 3);
        }

        /// <summary>
        /// Blends neighbor textures
        /// </summary>
        /// <param name="neighborGraphicIdList"></param>
        /// <param name="neighborGraphicIdsArr"></param>
        /// <param name="tile"></param>
        private void BlendNeighbors(List<int> neighborGraphicIdList, int[] neighborGraphicIdsArr, BaseObject tile)
        {
            foreach (var neighborId in neighborGraphicIdList)
            {
                var currentMeshData = GetMeshData(neighborId, false, (MeshFlags.Base | MeshFlags.Color));
                
                var vIndex = currentMeshData.vertices.Count;

                var z = GraphicInstance.instances[neighborId].Priority;

                BuildNeighborVertices(currentMeshData, tile, z);

                currentMeshData.colors.AddRange(SetNeighborColors(neighborId, neighborGraphicIdsArr));
                
                currentMeshData.AddTriangle(vIndex, 0, 8, 6);
                currentMeshData.AddTriangle(vIndex, 0, 6, 7);
                currentMeshData.AddTriangle(vIndex, 1, 8, 0);
                currentMeshData.AddTriangle(vIndex, 1, 2, 8);
                currentMeshData.AddTriangle(vIndex, 2, 4, 8);
                currentMeshData.AddTriangle(vIndex, 2, 3, 4);
                currentMeshData.AddTriangle(vIndex, 8, 5, 6);
                currentMeshData.AddTriangle(vIndex, 8, 4, 5);
            }
        }

        /// <summary>
        /// Sets colors for neighbor
        /// </summary>
        /// <param name="neighborId"></param>
        /// <param name="neighborGraphicIdsArr"></param>
        /// <returns>An array of colors</returns>
        private IEnumerable<Color> SetNeighborColors(int neighborId, IReadOnlyList<int> neighborGraphicIdsArr)
        {
            var colors = GetNewColorArray();

            for (var i = 0; i < 8; i++) 
            {
                if (i % 2 != 0) 
                { 
                    if (neighborId == neighborGraphicIdsArr[i]) 
                    {
                        colors[i] = Color.white;
                    }
                } 
                else 
                {
                    if (neighborId == neighborGraphicIdsArr[i]) 
                    {
                        switch (i)
                        {
                            case 4: // South
                                colors[3] = Color.white;
                                colors[4] = Color.white;
                                colors[5] = Color.white;
                                break;
                            case 6:  // West
                                colors[6] = Color.white;
                                colors[7] = Color.white;
                                colors[5] = Color.white;
                                break;
                            case 0: // North
                                colors[0] = Color.white;
                                colors[7] = Color.white;
                                colors[1] = Color.white;
                                break;
                            case 2: // East
                                colors[1] = Color.white;
                                colors[2] = Color.white;
                                colors[3] = Color.white;
                                break;
                        }
                    }
                }
            }

            return colors;
        }

        /// <summary>
        /// Gets a new clear color array
        /// </summary>
        /// <returns>An array of clear colors</returns>
        private static Color[] GetNewColorArray()
        {
            var colors = new Color[9]; 

            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }

            return colors;
        }
    }
}
