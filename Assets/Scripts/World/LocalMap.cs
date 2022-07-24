using GoRogue;
using GoRogue.GameFramework;
using UnityEngine;
using World.PlacedObjectTypes;

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
            var gridObject = GetGridObjectAt(gridPosition);

            return gridObject == null;
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
    }
}
