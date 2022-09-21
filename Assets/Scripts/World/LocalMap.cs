using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using Pathfinding;
using UnityEngine;
using Utilities;
using World.Pawns;
using World.Pawns.Jobs;
using Random = UnityEngine.Random;

namespace World
{
    /// <summary>
    /// The local map class
    /// </summary>
    /// <seealso cref="Map"/>
    public class LocalMap : Map
    {
        /// <summary>
        /// The number of entity layers
        /// </summary>
        private const int NumberOfEntityLayers = 3;
        
        /// <summary>
        /// The pawns
        /// </summary>
        private List<Pawn> _pawns;

        /// <summary>
        /// The on map changed
        /// </summary>
        public Action onMapChanged;
        
        /// <summary>
        /// The on job added
        /// </summary>
        public Action<Job> onJobAdded;

        public Dictionary<MapLayer, LayerGrid> layerGrids;

        public Coord Size => new Coord(Width, Height);

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMap"/> class
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        public LocalMap(int width, int height) : base(width, height, NumberOfEntityLayers, Distance.CHEBYSHEV)
        {
            Direction.YIncreasesUpward = true;

            _pawns = new List<Pawn>();

            layerGrids = new Dictionary<MapLayer, LayerGrid>();

            layerGrids.Add(MapLayer.Terrain, new GroundGrid(Size));
            
            layerGrids.Add(MapLayer.Plant, new InstancedGrid(Size, MapLayer.Plant));
            
            //todo other layers
        }

        public Vector3 GetMapCenterWithOffset()
        {
            return new Vector3(Width / 2.0f - 0.5f, Height / 2.0f - 0.5f);
        }

        public void UpdateVisibleBuckets()
        {
            var i = 0;

            foreach (var bucket in layerGrids[MapLayer.Terrain].Buckets)
            {
                var bucketVisible = bucket.CalcVisible();

                foreach (var grid in layerGrids.Values)
                {
                    if (grid.Layer != MapLayer.Terrain)
                    {
                        grid.Buckets[i].SetVisible(bucketVisible);
                    }
                }

                i++;
            }
        }

        public void BuildAllMeshes()
        {
            foreach (var layerGrid in layerGrids.Values)
            {
                layerGrid.BuildStaticMeshes();
            }
        }

        public void CheckAllMatrices()
        {
            foreach (var layerGrid in layerGrids.Values)
            {
                layerGrid.CheckMatricesUpdates();
            }
        }

        public void DrawBuckets()
        {
            foreach (var layerGrid in layerGrids.Values)
            {
                layerGrid.DrawBuckets();
            }
        }

        public void AddBaseObject(BaseObject baseObject, Coord position, bool force = false)
        {
            if (force || baseObject.Layer == (int) MapLayer.Undefined || GetBaseObjectAt(position, baseObject.GetMapLayer()) == null)
            {
                layerGrids[baseObject.GetMapLayer()].AddBaseObject(baseObject);
            }
        }

        public BaseObject GetBaseObjectAt(Coord position, MapLayer layer)
        {
            return layerGrids[layer].GetBaseObjectAt(position);
        }

        public IEnumerable<BaseObject> GetAllBaseObjectsAt(Coord position)
        {
            foreach (var layerGrid in layerGrids.Values)
            {
                var tile = layerGrid.GetBaseObjectAt(position);

                if (tile != null)
                {
                    yield return tile;
                }
            }
        }

        /// <summary>
        /// Gets the tile at using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The tile</returns>
        public Tile GetTileAt(Coord position) //todo is this needed?
        {
            return OutOfBounds(position) ? null : GetTerrain<Tile>(position);
        }

        /// <summary>
        /// Gets the random tile using the specified need walkable
        /// </summary>
        /// <param name="needWalkable">The need walkable</param>
        /// <returns>The tile</returns>
        public Tile GetRandomTile(bool needWalkable = false)
        {
            List<Tile> tilePool;

            if (needWalkable)
            {
                tilePool = GetAllWalkableTiles();
            }
            else
            {
                tilePool = GetAllTiles();
            }

            return tilePool[Random.Range(0, tilePool.Count)];
        }

        /// <summary>
        /// Gets the grid object at using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The grid object</returns>
        public GridObject GetGridObjectAt(Coord position)
        {
            return OutOfBounds(position) ? null : GetEntity<GridObject>(position);
        }

        /// <summary>
        /// Describes whether this instance can place grid object at
        /// </summary>
        /// <param name="gridPosition">The grid position</param>
        /// <returns>The bool</returns>
        public bool CanPlaceGridObjectAt(Coord gridPosition)
        {
            if (OutOfBounds(gridPosition))
            {
                return false;
            }
            
            var gridObject = GetGridObjectAt(gridPosition);

            return gridObject == null;
        }
        
        /// <summary>
        /// Places the placed object using the specified placed object
        /// </summary>
        /// <param name="placedObject">The placed object</param>
        public void PlacePlacedObject(PlacedObject placedObject)
        {
            foreach (var gridObject in placedObject.GridObjects)
            {
                PlaceGridObject(gridObject);
            }

            if (placedObject.placedObjectType.isWall)
            {
                UpdateNeighborWallTextures(placedObject.gridPositions.First().ToCoord());
            }
        }

        /// <summary>
        /// Gets the all pawns
        /// </summary>
        /// <returns>The pawns</returns>
        public List<Pawn> GetAllPawns()
        {
            return _pawns;
        }
        
        /// <summary>
        /// Places the pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="gridPosition">The grid position</param>
        public void PlacePawn(Pawn pawn, Coord gridPosition)
        {
            pawn.Position = gridPosition;
            
            var placed = AddEntity(pawn);

            if (!placed)
            {
                Debug.LogError($"Failed to place pawn at {pawn.Position.ToString()}");
            }
            
            _pawns ??= new List<Pawn>();
            
            _pawns.Add(pawn);
        }
        
        /// <summary>
        /// Removes the pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public void RemovePawn(Pawn pawn)
        {
            RemoveGridObject(pawn);
            
            _pawns.Remove(pawn);
        }

        /// <summary>
        /// Places the pawn at edge
        /// </summary>
        public void PlacePawnAtEdge()
        {
            //todo pick position from map edges
            
            //todo if not blocked then place pawn
            
            //todo otherwise pick another edge
        }

        /// <summary>
        /// Gets the all blueprints
        /// </summary>
        /// <returns>The blue prints</returns>
        public List<PlacedObject> GetAllBlueprints()
        {
            var bluePrints = new List<PlacedObject>();
            
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var coord = new Coord(x, y);
                    
                    var gridObject = GetBlueprintAt(coord);

                    if (gridObject == null)
                    {
                        continue;
                    }
                    
                    if (bluePrints.Contains(gridObject.PlacedObject))
                    {
                        continue;
                    }
                    
                    bluePrints.Add(gridObject.PlacedObject);
                }
            }

            return bluePrints;
        }
        
        /// <summary>
        /// Describes whether this instance walkable at
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The bool</returns>
        public bool WalkableAt(Coord position)
        {
            var tile = GetTileAt(position);

            if (tile == null)
            {
                return false;
            }

            if (!tile.IsWalkable)
            {
                return false;
            }

            var objects = GetObjects(position);

            foreach (var iGameObject in objects)
            {
                if (!iGameObject.IsWalkable)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the blueprint at using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The grid object</returns>
        public GridObject GetBlueprintAt(Coord position)
        {
            var gridObject = GetGridObjectAt(position);

            if (gridObject == null)
            {
                return null;
            }

            return gridObject.IsBlueprint() ? gridObject : null;
        }
        
        /// <summary>
        /// Describes whether this instance out of bounds
        /// </summary>
        /// <param name="targetCoord">The target coord</param>
        /// <returns>The bool</returns>
        private bool OutOfBounds(Coord targetCoord)
        {
            var (x, y) = targetCoord;

            return x >= Width || x < 0 || y >= Height || y < 0;
        }
        
        /// <summary>
        /// Places the grid object using the specified grid object
        /// </summary>
        /// <param name="gridObject">The grid object</param>
        private void PlaceGridObject(GridObject gridObject)
        {
            var placed = AddEntity(gridObject);

            if (!placed)
            {
                Debug.LogError($"Failed to place object at {gridObject.Position.ToString()}");
            }

            UpdateAStar(gridObject);

            //todo else notify map changed
            
            //todo if needs to be built, create a job and add to job giver -- could be part of map changed event
            //have job giver sub to map changed event

            if (!gridObject.IsBlueprint())
            {
                return;
            }
            
            var placedObjectType = gridObject.PlacedObject.placedObjectType;

            var job = new Job(gridObject.Position, placedObjectType.skill, placedObjectType.minSkillLevel);
            
            onJobAdded?.Invoke(job);
        }
        
        /// <summary>
        /// Removes the grid object using the specified grid object
        /// </summary>
        /// <param name="gridObject">The grid object</param>
        private void RemoveGridObject(IGameObject gridObject)
        {
            var removed = RemoveEntity(gridObject);
            
            if (!removed)
            {
                Debug.LogError("Failed to remove object from local map!");
            }
            
            UpdateAStar(gridObject);
        }

        public LayerGridBucket GetBucketAt(Coord coord, MapLayer layer = MapLayer.Terrain)
        {
            return layerGrids[layer].GetBucketAt(coord);
        }

        public void UpdateAStar(IGameObject gridObject)
        {
            var bucket = layerGrids.First().Value.GetBucketAt(gridObject.Position);

            UpdateAStar(bucket);
        }

        public void UpdateAStar(LayerGridBucket bucket)
        {
            var bounds = new Bounds(bucket.Rect.Center.ToVector3(), bucket.Rect.Size.ToVector3());
            
            AstarPath.active.UpdateGraphs(new GraphUpdateObject(bounds));
        }
        
        /// <summary>
        /// Updates the neighbor wall textures using the specified coord
        /// </summary>
        /// <param name="coord">The coord</param>
        private void UpdateNeighborWallTextures(Coord coord)
        {
            foreach (var directionType in CollectionUtils.EnumToArray<Direction.Types>())
            {
                UpdateNeighborWallTexture(coord, Direction.ToDirection(directionType));
            }
        }

        /// <summary>
        /// Gets the all neighbors using the specified coord
        /// </summary>
        /// <param name="coord">The coord</param>
        /// <returns>A list of coord</returns>
        private List<Coord> GetAllNeighbors(Coord coord)
        {
            var rule = AdjacencyRule.ToAdjacencyRule(AdjacencyRule.EIGHT_WAY.Type);

            return rule.Neighbors(coord).ToList();
        }

        /// <summary>
        /// Updates the neighbor wall texture using the specified coord
        /// </summary>
        /// <param name="coord">The coord</param>
        /// <param name="direction">The direction</param>
        private void UpdateNeighborWallTexture(Coord coord, Direction direction)
        {
            var neighbor = GetGridObjectAt(coord + direction);
            
            if (neighbor != null && neighbor.PlacedObject.placedObjectType.isWall)
            {
                ((WallPlacedObject)neighbor.PlacedObject).UpdateTexture();
            }
        }

        /// <summary>
        /// Gets the all walkable tiles
        /// </summary>
        /// <returns>A list of tile</returns>
        private List<Tile> GetAllWalkableTiles()
        {
            return GetAllTiles().Where(t => WalkableAt(t.Position)).ToList();
        }
        
        /// <summary>
        /// Gets the all tiles
        /// </summary>
        /// <returns>The tiles</returns>
        private List<Tile> GetAllTiles()
        {
            var tiles = new List<Tile>();
            
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var tile = GetTileAt(new Coord(x, y));
                    
                    tiles.Add(tile);
                }
            }
            
            return tiles;
        }

        /// <summary>
        /// Gets the adjacent walkable locations using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>An enumerable of coord</returns>
        public IEnumerable<Coord> GetAdjacentWalkableLocations(Coord position)
        {
            return GetAllNeighbors(position).Where(WalkableAt).ToList();
        }
    }
}
