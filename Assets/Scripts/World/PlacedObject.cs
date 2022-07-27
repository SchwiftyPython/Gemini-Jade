using System.Collections.Generic;
using UnityEngine;
using World.Things.CraftableThings;

namespace World
{
    public class PlacedObject : MonoBehaviour
    {
        public static PlacedObject Create(Vector2Int origin, Dir direction, PlacedObjectTemplate placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();

            var placedObjectInstance = Instantiate(placedObjectType.Prefab,
                gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<PlacedObject>();
            
            placedObject.placedObjectType = placedObjectType;

            MapLayer layer;
            bool walkable;
            bool transparent;

            placedObject.remainingWork = placedObjectType.workToMake;

            if (placedObject.NeedsToBeMade)
            {
                placedObject.spriteRenderer.sprite = placedObjectType.blueprintTexture;

                walkable = true;
                
                transparent = true;
            }
            else
            {
                placedObject.spriteRenderer.sprite = placedObjectType.texture;

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

            return placedObject;
        }

        public enum Dir 
        {
            Down,
            Left,
            Up,
            Right,
        }
        
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected PlacedObjectTemplate placedObjectType;

        protected List<Vector3> gridPositions;
        
        protected Dir direction;

        protected int remainingWork;
        
        public List<GridObject> GridObjects { get; private set; }
        
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        public bool NeedsToBeMade => remainingWork > 0;

        public List<Vector3> GetGridPositions(Vector2Int origin, Dir dir)
        {
            var gridPositionList = new List<Vector3>();
            
            switch (dir)
            {
                default:
                case Dir.Down:
                    for (var x = 0; x < placedObjectType.width; x++)
                    {
                        for (var y = 0; y < placedObjectType.height; y++)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) + new Vector3(x, y));
                        }
                    }

                    break;
                
                case Dir.Up:
                    for (var x = placedObjectType.width - 1; x >= 0; x--)
                    {
                        for (var y = placedObjectType.height - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) - new Vector3(x, y));
                        }
                    }

                    break;
                
                case Dir.Left:
                    for (var x = 0; x < placedObjectType.height; x++)
                    {
                        for (var y = 0; y < placedObjectType.width; y++)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) + new Vector3(x, y));
                        }
                    }

                    break;
                
                case Dir.Right:
                    for (var x = placedObjectType.height - 1; x >= 0; x--)
                    {
                        for (var y = placedObjectType.width - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) - new Vector3(x, y));
                        }
                    }
                    
                    break;
            }

            return gridPositionList;
        }

        public void Make()
        {
            SpriteRenderer.sprite = placedObjectType.texture;

            foreach (var gridObject in GridObjects)
            {
                gridObject.IsWalkable = placedObjectType.walkable;
                
                gridObject.IsTransparent = placedObjectType.transparent;
            }
        }
    }
}
