using System.Collections.Generic;
using UnityEngine;
using World.PlacedObjectTypes;

namespace World
{
    public class PlacedObject : MonoBehaviour
    {
        public static PlacedObject Create(Vector3 gridPosition, Dir direction, PlacedObjectType placedObjectType)
        {
            var placedObjectInstance = Instantiate(placedObjectType.prefab, gridPosition, Quaternion.Euler(0, 0, 0));
            
            var placedObject = placedObjectInstance.GetComponent<PlacedObject>();

            placedObject.spriteRenderer.sprite = placedObjectType.texture;

            placedObject.placedObjectType = placedObjectType;
            
            placedObject.gridPosition = gridPosition;
            
            placedObject.direction = direction;

            placedObject.GridObject =
                new GridObject(gridPosition, true, placedObjectType.walkable, placedObjectType.transparent);

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

        protected Vector3 gridPosition;
        
        protected Dir direction;
        
        public GridObject GridObject { get; private set; }

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

        public List<Vector2Int> GetGridPositions(Vector2Int offset, Dir dir)
        {
            var gridPositionList = new List<Vector2Int>();
            
            switch (dir)
            {
                default:
                case Dir.Down:
                case Dir.Up:
                    for (var x = 0; x < placedObjectType.width; x++)
                    {
                        for (var y = 0; y < placedObjectType.height; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }

                    break;
                
                case Dir.Left:
                case Dir.Right:
                    for (var x = 0; x < placedObjectType.height; x++)
                    {
                        for (var y = 0; y < placedObjectType.width; y++)
                        {
                            gridPositionList.Add(offset + new Vector2Int(x, y));
                        }
                    }

                    break;
            }

            return gridPositionList;
        }
    }
}
