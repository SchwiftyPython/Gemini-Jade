using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using Utilities;

namespace World
{
    public class LocalMap : Map
    {
        public LocalMap(int width, int height) : base(width, height, 1, Distance.CHEBYSHEV)
        {
            Direction.YIncreasesUpward = true;
        }
        
        public bool OutOfBounds(Coord targetCoord)
        {
            var (x, y) = targetCoord;

            return x >= Width || x < 0 || y >= Height || y < 0;
        }
        
        public Tile GetTileAt(Coord position)
        {
            return OutOfBounds(position) ? null : GetTerrain<Tile>(position);
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
        
        public void PlaceGridObjectAt(Coord gridPosition, GridObject gridObject)
        {
            gridObject.Position = gridPosition;

            var placed = AddEntity(gridObject);

            if (placed)
            {
                Debug.Log($"Placed object at {gridPosition}");
            }
            else
            {
                Debug.Log($"Failed to place object at {gridPosition}");
            }
        }

        public List<PlacedObject> GetAllBlueprints()
        {
            var bluePrints = new List<PlacedObject>();
            
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var coord = new Coord(x, y);
                    
                    var gridObject = GetGridObjectAt(coord);

                    if (gridObject == null)
                    {
                        continue;
                    }

                    if (!gridObject.IsBlueprint())
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
        
        private void PlaceGridObject(IGameObject gridObject)
        {
            var placed = AddEntity(gridObject);

            if (placed)
            {
                Debug.Log($"Placed object at {gridObject.Position.ToString()}");
            }
            else
            {
                Debug.Log($"Failed to place object at {gridObject.Position.ToString()}");
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
