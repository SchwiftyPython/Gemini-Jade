using System.Collections.Generic;
using GoRogue;
using UnityEngine;
using Utilities;
using World;

namespace Graphics
{
    public class BucketGroundRenderer : BucketRenderer
    {
        public BucketGroundRenderer(LayerGridBucket bucket, MapLayer layer) : base(bucket, layer)
        {
        }

        public override void BuildMeshes()
        {
            //todo this method is dumb long yo
            
            var neighborGraphicIdList = new List<int>();

            var neighborGraphicIdsArr = new int[8];

            var colors = new Color[9]; //todo why 9?

            foreach (var tile in Bucket.Tiles)
            {
                if (tile.hidden)
                {
                    continue;
                }
                
                neighborGraphicIdList.Clear();
                
                MeshData currentMeshData = GetMeshData(tile.MainGraphic.Uid, false, (MeshFlags.Base | MeshFlags.Color));
                
                int vIndex = currentMeshData.vertices.Count;
                
                float z = tile.MainGraphic.Priority;

                currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y, z));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y+1, z));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X+1, tile.Position.Y+1, z));
                currentMeshData.vertices.Add(new Vector3(tile.Position.X+1, tile.Position.Y, z));
                currentMeshData.colors.Add(Color.white);
                currentMeshData.colors.Add(Color.white);
                currentMeshData.colors.Add(Color.white);
                currentMeshData.colors.Add(Color.white);
                currentMeshData.AddTriangle(vIndex, 0, 1, 2);
                currentMeshData.AddTriangle(vIndex, 0, 2, 3);

                foreach (var directionType in CollectionUtils.EnumToArray<Direction.Types>())
                {
                    var neighbor = tile.GetAdjacentTileByDirection(Direction.ToDirection(directionType));

                    if (neighbor != null)
                    {
                        neighborGraphicIdsArr[(int) directionType] = neighbor.MainGraphic.Uid;

                        if (!neighborGraphicIdList.Contains(neighbor.MainGraphic.Uid) &&
                            neighbor.MainGraphic.Uid !=
                            tile.MainGraphic.Uid) //todo && neighbor.TileType.MaxHeight >= tile.TileType.MaxHeight
                        {
                            neighborGraphicIdList.Add(neighbor.MainGraphic.Uid);
                        }
                    }
                    else
                    {
                        neighborGraphicIdsArr[(int) directionType] = tile.MainGraphic.Uid;
                    }
                }

                foreach (var neighborId in neighborGraphicIdList)
                {
                    currentMeshData = GetMeshData(neighborId, false, (MeshFlags.Base | MeshFlags.Color));
                
                    vIndex = currentMeshData.vertices.Count;

                    z = GraphicInstance.instances[neighborId].Priority;
                    
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y, z));         // 0
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y, z));               // 1
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y + .5f, z));         // 2
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X, tile.Position.Y + 1, z));           // 3
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y + 1, z));     // 4
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + 1, tile.Position.Y + 1, z));       // 5
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + 1, tile.Position.Y + .5f, z));     // 6
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + 1, tile.Position.Y, z));           // 7
                    currentMeshData.vertices.Add(new Vector3(tile.Position.X + .5f, tile.Position.Y + .5f, z));   // 8

                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = Color.clear;
                    }
                    
                    for (int i = 0; i < 8; i++) 
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
                                    case 0: // South
                                        colors[1] = Color.white;
                                        colors[0] = Color.white;
                                        colors[7] = Color.white;
                                        break;
                                    case 3:  // West
                                        colors[1] = Color.white;
                                        colors[2] = Color.white;
                                        colors[3] = Color.white;
                                        break;
                                    case 6: // North
                                        colors[3] = Color.white;
                                        colors[4] = Color.white;
                                        colors[5] = Color.white;
                                        break;
                                    case 5: // East
                                        colors[5] = Color.white;
                                        colors[6] = Color.white;
                                        colors[7] = Color.white;
                                        break;
                                }
                            }
                        }
                    }
                    
                    currentMeshData.colors.AddRange(colors);
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

            foreach (var meshData in Meshes.Values)
            {
                meshData.Build();
            }
        }
    }
}
