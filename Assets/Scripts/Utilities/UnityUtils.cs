using System;
using GoRogue;
using Pathfinding;
using UnityEngine;
using World;
using World.Pawns;
using Object = UnityEngine.Object;

namespace Utilities
{
    /// <summary>
    /// The unity utils class
    /// </summary>
    public static class UnityUtils
    {
        /// <summary>
        /// Destroys the all children using the specified parent
        /// </summary>
        /// <param name="parent">The parent</param>
        public static void DestroyAllChildren(this GameObject parent)
        {
            for (var i = 0; i < parent.transform.childCount; i++)
            {
                Object.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        
        /// <summary>
        /// Returns the coord using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The coord</returns>
        public static Coord ToCoord(this Vector2Int position)
        {
            return new Coord(position.x, position.y);
        }
        
        /// <summary>
        /// Returns the coord using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The coord</returns>
        public static Coord ToCoord(this Vector3 position)
        {
            var coord = new Coord(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            return coord;
        }

        /// <summary>
        /// Returns the vector 3 using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The vector</returns>
        public static Vector3 ToVector3(this Vector2Int position)
        {
            return new Vector3(position.x, position.y);
        }
        
        /// <summary>
        /// Returns the vector 3 using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The vector</returns>
        public static Vector3 ToVector3(this Coord position)
        {
            return new Vector3(position.X, position.Y, 0);
        }
        
        /// <summary>
        /// Returns the vector 2 int using the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The vector int</returns>
        public static Vector2Int ToVector2Int(this Vector3 position)
        {
            return new Vector2Int((int) position.x, (int) position.y);
        }

        /// <summary>
        /// Returns the map direction using the specified bit mask direction
        /// </summary>
        /// <param name="bitMaskDirection">The bit mask direction</param>
        /// <exception cref="ArgumentOutOfRangeException">null</exception>
        /// <returns>The direction</returns>
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
        
        /// <summary>
        /// Adds the box collider to using the specified game object
        /// </summary>
        /// <param name="gameObject">The game object</param>
        public static void AddBoxColliderTo(GameObject gameObject)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        /// <summary>
        /// Adds the pathfinding to using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <param name="gameObject">The game object</param>
        public static void AddPathfindingTo(Pawn pawn, GameObject gameObject)
        {
            gameObject.AddComponent<Seeker>();

            gameObject.AddComponent<PawnMovement>();
            
            gameObject.GetComponent<PawnMovement>().Init(pawn);

            gameObject.AddComponent<SimpleSmoothModifier>();
            
            gameObject.GetComponent<SimpleSmoothModifier>().maxSegmentLength = 1;
            
            gameObject.GetComponent<SimpleSmoothModifier>().iterations = 5;
            
            gameObject.GetComponent<SimpleSmoothModifier>().strength = 0.05f;

            gameObject.AddComponent<RaycastModifier>();

            gameObject.GetComponent<RaycastModifier>().use2DPhysics = true;

            gameObject.GetComponent<RaycastModifier>().thickRaycast = true;

            gameObject.GetComponent<RaycastModifier>().thickRaycastRadius = 2;
        }
    }
}
