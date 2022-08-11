using System;
using GoRogue;
using Pathfinding;
using UnityEngine;
using World;
using World.Pawns;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class UnityUtils
    {
        public static void DestroyAllChildren(this GameObject parent)
        {
            for (var i = 0; i < parent.transform.childCount; i++)
            {
                Object.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        
        public static Coord ToCoord(this Vector2Int position)
        {
            return new Coord(position.x, position.y);
        }
        
        public static Coord ToCoord(this Vector3 position)
        {
            return new Coord((int) position.x, (int) position.y);
        }

        public static Vector3 ToVector3(this Vector2Int position)
        {
            return new Vector3(position.x, position.y);
        }
        
        public static Vector3 ToVector3(this Coord position)
        {
            return new Vector3(position.X, position.Y, 0);
        }
        
        public static Vector2Int ToVector2Int(this Vector3 position)
        {
            return new Vector2Int((int) position.x, (int) position.y);
        }

        public static Direction ToMapDirection(this BitMaskDirection bitMaskDirection)
        {
            return bitMaskDirection switch
            {
                BitMaskDirection.NorthWest => Direction.UP_LEFT,
                BitMaskDirection.North => Direction.UP,
                BitMaskDirection.NorthEast => Direction.UP_RIGHT,
                BitMaskDirection.West => Direction.LEFT,
                BitMaskDirection.East => Direction.RIGHT,
                BitMaskDirection.SouthWest => Direction.DOWN_LEFT,
                BitMaskDirection.South => Direction.DOWN,
                BitMaskDirection.SouthEast => Direction.DOWN_RIGHT,
                _ => throw new ArgumentOutOfRangeException(nameof(bitMaskDirection), bitMaskDirection, null)
            };
        }
        
        public static void AddBoxColliderTo(GameObject gameObject)
        {
            gameObject.AddComponent<BoxCollider2D>();
            
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0.5f);
        }
        
        public static void AddPathfindingTo(Pawn pawn, GameObject gameObject)
        {
            gameObject.AddComponent<Seeker>();

            gameObject.AddComponent<PawnMovement>();
            
            gameObject.GetComponent<PawnMovement>().Init(pawn);

            // gameObject.AddComponent<SimpleSmoothModifier>();
            //
            // gameObject.GetComponent<SimpleSmoothModifier>().maxSegmentLength = 1;
            //
            // gameObject.GetComponent<SimpleSmoothModifier>().iterations = 5;
            //
            // gameObject.GetComponent<SimpleSmoothModifier>().strength = 0.25f;

            gameObject.AddComponent<RaycastModifier>();

            gameObject.GetComponent<RaycastModifier>().use2DPhysics = true;
            
            
        }
    }
}
