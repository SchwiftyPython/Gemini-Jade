using System.Collections.Generic;
using System.Linq;
using Repos;
using UnityEngine;
using World.Things.CraftableThings;

namespace World
{
    public class WallPlacedObject : PlacedObject
    {
        public static PlacedObject Create(Vector2Int origin, Dir direction, PlacedObjectTemplate placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();

            var placedObjectInstance = Instantiate(placedObjectType.Prefab,
                gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<WallPlacedObject>();
            
            placedObject.placedObjectType = placedObjectType;

            MapLayer layer;
            bool walkable;
            bool transparent;

            placedObject.remainingWork = placedObjectType.workToMake;
            
            //todo find correct texture and update neighbor tiles or trigger grid changed if we go beyond neighbors

            if (placedObject.NeedsToBeMade)
            {
                walkable = true;
                
                transparent = true;
            }
            else
            {
                walkable = placedObjectType.walkable;
                
                transparent = placedObjectType.transparent;
            }

            placedObject.direction = direction;

            placedObject.GridObjects = new List<GridObject>();

            placedObject.gridPositions = placedObject.GetGridPositions(origin, direction);

            foreach (var position in placedObject.gridPositions)
            {
                var gridObject = new GridObject(placedObject, position, true, walkable,
                    transparent);

                placedObject.GridObjects.Add(gridObject);
            }
            
            placedObject.UpdateTexture();

            return placedObject;
        }

        public void UpdateTexture()
        {
            //todo check if blueprint or not
            
            var tileIndex = GetTileIndex();
            
            Debug.Log($"Tile Index for wall: {tileIndex}");
            
            var buildablesRepo = FindObjectOfType<BuildablesRepo>();

            if (NeedsToBeMade)
            {
                //todo get blueprint sprite
                SpriteRenderer.sprite = buildablesRepo.GetWallSpriteAt(tileIndex);
                
                SpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            else
            {
                SpriteRenderer.sprite = buildablesRepo.GetWallSpriteAt(tileIndex);
                
                SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            }
        }

        public override void Make()
        {
            remainingWork = 0;
            
            UpdateTexture();

            GridObjects.First().IsWalkable = placedObjectType.walkable;
            
            GridObjects.First().IsTransparent = placedObjectType.transparent;
        }

        private int GetTileIndex()
        {
            var east = NeighborTo(BitMaskDirection.East);
            
            var west = NeighborTo(BitMaskDirection.West);
            
            var north = NeighborTo(BitMaskDirection.North);
            
            var south = NeighborTo(BitMaskDirection.South);
            
            var northWest = NeighborTo(BitMaskDirection.NorthWest);
            
            var northEast = NeighborTo(BitMaskDirection.NorthEast);
            
            var southWest = NeighborTo(BitMaskDirection.SouthWest);
            
            var southEast = NeighborTo(BitMaskDirection.SouthEast);

            return GridBuildingSystem.CalculateTileIndex(east, west, north, south, northWest, northEast, southWest, southEast);
        }
        
        private bool NeighborTo(BitMaskDirection direction)
        {
            return GridObjects.First().HasWallNeighborTo(direction);
        }
    }
}
