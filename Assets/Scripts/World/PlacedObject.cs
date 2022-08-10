using System.Collections.Generic;
using UnityEngine;
using Utilities;
using World.Things.CraftableThings;

namespace World
{
    public class PlacedObject : MonoBehaviour
    {
        public static readonly Color BlueprintColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        public static readonly Color BuiltColor = new Color(1f, 1f, 1f, 1f);
        
        public static PlacedObject Create(Vector2Int origin, Dir direction, PlacedObjectTemplate placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();

            var placedObjectInstance = Instantiate(placedObjectType.Prefab,
                gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<PlacedObject>();

            placedObject.instance = placedObjectInstance;
            
            placedObject.placedObjectType = placedObjectType;

            MapLayer layer;
            bool walkable;
            bool transparent;

            placedObject.remainingWork = placedObjectType.workToMake;

            if (placedObject.NeedsToBeMade)
            {
                placedObject.SpriteRenderer.sprite = placedObjectType.blueprintTexture;
                
                placedObject.SpriteRenderer.color = BlueprintColor;

                walkable = true;
                
                transparent = true;
            }
            else
            {
                placedObject.spriteRenderer.sprite = placedObjectType.builtTexture;
                
                placedObject.SpriteRenderer.color = BuiltColor;

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

            return placedObject;
        }

        public enum Dir 
        {
            Down,
            Left,
            Up,
            Right,
        }

        protected Transform instance;
        
        [SerializeField] protected internal SpriteRenderer spriteRenderer;

        protected internal PlacedObjectTemplate placedObjectType;

        protected internal List<Vector3> gridPositions;
        
        protected Dir direction;

        protected int remainingWork;
        
        public List<GridObject> GridObjects { get; internal set; }
        
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

        public virtual void Make()
        {
            remainingWork = 0;
            
            SpriteRenderer.sprite = placedObjectType.builtTexture;
            
            SpriteRenderer.color = BuiltColor;

            foreach (var gridObject in GridObjects)
            {
                gridObject.IsWalkable = placedObjectType.walkable;
                
                gridObject.IsTransparent = placedObjectType.transparent;
            }
            
            if (!placedObjectType.walkable)
            {
                UnityUtils.AddBoxColliderTo(instance.gameObject);
            }
        }
    }
}
