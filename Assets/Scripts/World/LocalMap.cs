using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;
using World.Pawns;

namespace World
{
    public class LocalMap : Map
    {
        private const int NumberOfEntityLayers = 3;
        
        private List<Pawn> _pawns;
        
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
            var coord = new Coord(Random.Range(0, Width), Random.Range(0, Height));
            
            var tile = GetTileAt(coord);
            
            if(!needWalkable)
            {
                return tile;
            }

            while (!tile.IsWalkable) //todo would be safer to get all walkable tiles from map
            {
                coord = new Coord(Random.Range(0, Width), Random.Range(0, Height));
                
                tile = GetTileAt(coord);
            }

            return tile;
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
            
            PlaceGridObject(pawn);
            
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
        
        private GridObject GetBlueprintAt(Coord position)
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
        
        private void PlaceGridObject(IGameObject gridObject)
        {
            var placed = AddEntity(gridObject);

            if (!placed)
            {
                Debug.LogError($"Failed to place object at {gridObject.Position.ToString()}");
            }
        }
        
        private void RemoveGridObject(IGameObject gridObject)
        {
            var removed = RemoveEntity(gridObject);
            
            if (!removed)
            {
                Debug.LogError("Failed to remove object from local map!");
            }
        }
        
        private void UpdateNeighborWallTextures(Coord coord)
        {
            foreach (var directionType in CollectionUtils.EnumToArray<Direction.Types>())
            {
                UpdateNeighborWallTexture(coord, Direction.ToDirection(directionType));
            }
        }

        private void UpdateNeighborWallTexture(Coord coord, Direction direction)
        {
            var neighbor = GetGridObjectAt(coord + direction);
            
            if (neighbor != null && neighbor.PlacedObject.placedObjectType.isWall)
            {
                ((WallPlacedObject)neighbor.PlacedObject).UpdateTexture();
            }
        }
    }
}
