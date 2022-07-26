using System.Collections.Generic;
using UnityEngine;
using Utilities;
using World.PlacedObjectTypes;

namespace World
{
    public class PlacedObject : MonoBehaviour
    {
        public static PlacedObject Create(Vector2Int origin, Dir direction, PlacedObjectType placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
            
            var placedObjectInstance = Instantiate(placedObjectType.prefab,  gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<PlacedObject>();
            
            //todo need to check if it has a blueprint. If not skip the blueprint texture

            placedObject.spriteRenderer.sprite = placedObjectType.texture;

            placedObject.placedObjectType = placedObjectType;

            placedObject.direction = direction;

            placedObject.GridObjects = new List<GridObject>();
            
            placedObject.gridPositions = placedObject.GetGridPositions(origin, direction);

            foreach (var position in placedObject.gridPositions)
            {
                var gridObject = new GridObject(placedObject, position, true, placedObjectType.walkable, placedObjectType.transparent);
                
                placedObject.GridObjects.Add(gridObject);
            }

            return placedObject;
        }

        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return Dir.Left;
                case Dir.Left: return Dir.Up;
                case Dir.Up: return Dir.Right;
                case Dir.Right: return Dir.Down;
            }
        }

        public static Vector2Int GetDirForwardVector(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return new Vector2Int(0, -1);
                
                case Dir.Left: return new Vector2Int(-1, 0);
                
                case Dir.Up: return new Vector2Int(0, +1);
                
                case Dir.Right: return new Vector2Int(+1, 0);
            }
        }

        public static Dir GetDir(Vector2Int from, Vector2Int to)
        {
            if (from.x < to.x)
            {
                return Dir.Right;
            }

            if (from.x > to.x)
            {
                return Dir.Left;
            }

            return from.y < to.y ? Dir.Up : Dir.Down;
        }

        public enum Dir 
        {
            Down,
            Left,
            Up,
            Right,
        }
        
        [SerializeField] protected SpriteRenderer spriteRenderer;

        protected PlacedObjectType placedObjectType;

        protected List<Vector3> gridPositions;
        
        protected Dir direction;
        
        public List<GridObject> GridObjects { get; private set; }
        
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        public int GetRotationAngle(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return 0;
                
                case Dir.Left: return 90;
                
                case Dir.Up: return 180;
                
                case Dir.Right: return 270;
            }
        }

        public Vector2Int GetRotationOffset(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Down: return new Vector2Int(0, 0);
                
                case Dir.Left: return new Vector2Int(0, placedObjectType.width);
                
                case Dir.Up: return new Vector2Int(placedObjectType.width, placedObjectType.height);
                
                case Dir.Right: return new Vector2Int(placedObjectType.height, 0);
            }
        }

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
    }
}
