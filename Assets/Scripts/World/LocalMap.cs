using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using World.Pawns;
using World.Pawns.Jobs;
using Random = UnityEngine.Random;

namespace World
{
    public class LocalMap : Map
    {
        private const int NumberOfEntityLayers = 3;
        
        private List<Pawn> _pawns;

        public Action onMapChanged;
        
        public Action<Job> onJobAdded;

        public LocalMap(int width, int height) : base(width, height, NumberOfEntityLayers, Distance.CHEBYSHEV)
        {
            Direction.YIncreasesUpward = true;
        }

        public Tile GetTileAt(Coord position)
        {
            return OutOfBounds(position) ? null : GetTerrain<Tile>(position);
        }

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

        public GridObject GetGridObjectAt(Coord position)
        {
            return OutOfBounds(position) ? null : GetEntity<GridObject>(position);
        }

        public bool CanPlaceGridObjectAt(Coord gridPosition)
        {
            if (OutOfBounds(gridPosition))
            {
                return false;
            }
            
            var gridObject = GetGridObjectAt(gridPosition);

            return gridObject == null;
        }
        
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

        public List<Pawn> GetAllPawns()
        {
            return _pawns;
        }
        
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
        
        public void RemovePawn(Pawn pawn)
        {
            RemoveGridObject(pawn);
            
            _pawns.Remove(pawn);
        }

        public void PlacePawnAtEdge()
        {
            //todo pick position from map edges
            
            //todo if not blocked then place pawn
            
            //todo otherwise pick another edge
        }

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

        public GridObject GetBlueprintAt(Coord position)
        {
            var gridObject = GetGridObjectAt(position);

            if (gridObject == null)
            {
                return null;
            }

            return gridObject.IsBlueprint() ? gridObject : null;
        }
        
        private bool OutOfBounds(Coord targetCoord)
        {
            var (x, y) = targetCoord;

            return x >= Width || x < 0 || y >= Height || y < 0;
        }
        
        private void PlaceGridObject(GridObject gridObject)
        {
            var placed = AddEntity(gridObject);

            if (!placed)
            {
                Debug.LogError($"Failed to place object at {gridObject.Position.ToString()}");
            }
            
            AstarPath.active.Scan();
            
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
        
        private void RemoveGridObject(IGameObject gridObject)
        {
            var removed = RemoveEntity(gridObject);
            
            if (!removed)
            {
                Debug.LogError("Failed to remove object from local map!");
            }
            
            AstarPath.active.Scan();
        }
        
        private void UpdateNeighborWallTextures(Coord coord)
        {
            foreach (var directionType in CollectionUtils.EnumToArray<Direction.Types>())
            {
                UpdateNeighborWallTexture(coord, Direction.ToDirection(directionType));
            }
        }

        private List<Coord> GetAllNeighbors(Coord coord)
        {
            var rule = AdjacencyRule.ToAdjacencyRule(AdjacencyRule.EIGHT_WAY.Type);

            return rule.Neighbors(coord).ToList();
        }

        private void UpdateNeighborWallTexture(Coord coord, Direction direction)
        {
            var neighbor = GetGridObjectAt(coord + direction);
            
            if (neighbor != null && neighbor.PlacedObject.placedObjectType.isWall)
            {
                ((WallPlacedObject)neighbor.PlacedObject).UpdateTexture();
            }
        }

        private List<Tile> GetAllWalkableTiles()
        {
            return GetAllTiles().Where(t => WalkableAt(t.Position)).ToList();
        }
        
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

        public IEnumerable<Coord> GetAdjacentWalkableLocations(Coord position)
        {
            return GetAllNeighbors(position).Where(WalkableAt).ToList();
        }
    }
}
