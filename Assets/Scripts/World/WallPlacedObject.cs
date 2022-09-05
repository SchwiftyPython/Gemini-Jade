using System.Collections.Generic;
using System.Linq;
using Repos;
using UnityEngine;
using Utilities;
using World.Things.CraftableThings;

namespace World
{
    /// <summary>
    /// The wall placed object class
    /// </summary>
    /// <seealso cref="PlacedObject"/>
    public class WallPlacedObject : PlacedObject
    {
        /// <summary>
        /// Creates the origin
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="direction">The direction</param>
        /// <param name="placedObjectType">The placed object type</param>
        /// <returns>The placed object</returns>
        public static PlacedObject Create(Vector2Int origin, Dir direction, PlacedObjectTemplate placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();

            var placedObjectInstance = Instantiate(placedObjectType.Prefab,
                gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<WallPlacedObject>();

            placedObject.instance = placedObjectInstance;
            
            placedObject.placedObjectType = placedObjectType;

            placedObject.map = gridBuildingSystem.LocalMap;

            MapLayer layer;
            bool walkable;
            bool transparent;

            placedObject.remainingWork = placedObjectType.workToMake;

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

            if (!walkable )
            {
                UnityUtils.AddBoxColliderTo(placedObjectInstance.gameObject);
            }
            
            placedObject.UpdateTexture();

            return placedObject;
        }

        /// <summary>
        /// Updates the texture
        /// </summary>
        public void UpdateTexture()
        {
            var tileIndex = GetTileIndex();

            var buildablesRepo = FindObjectOfType<BuildablesRepo>();
            
            SpriteRenderer.sprite = buildablesRepo.GetWallSpriteAt(tileIndex);

            SpriteRenderer.color = NeedsToBeMade ? BlueprintColor : BuiltColor;
        }

        /// <summary>
        /// Finishes the construction
        /// </summary>
        public override void FinishConstruction()
        {
            remainingWork = 0;
            
            MovePawnsOutTheWay();
            
            UpdateTexture();

            GridObjects.First().IsWalkable = placedObjectType.walkable;
            
            GridObjects.First().IsTransparent = placedObjectType.transparent;

            if (placedObjectType.walkable)
            {
                return;
            }

            UnityUtils.AddBoxColliderTo(instance.gameObject);
                
            map.UpdateAStar(GridObjects.First());
        }

        /// <summary>
        /// Gets the tile index
        /// </summary>
        /// <returns>The int</returns>
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
        
        /// <summary>
        /// Describes whether this instance neighbor to
        /// </summary>
        /// <param name="bmDirection">The bm direction</param>
        /// <returns>The bool</returns>
        private bool NeighborTo(BitMaskDirection bmDirection)
        {
            return GridObjects.First().HasWallNeighborTo(bmDirection);
        }
    }
}
