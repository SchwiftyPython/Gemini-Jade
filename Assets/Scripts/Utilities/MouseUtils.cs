using UnityEngine;
using UnityEngine.InputSystem;

namespace Utilities
{
    /// <summary>
    /// The mouse utils class
    /// </summary>
    public static class MouseUtils
    {
        /// <summary>
        /// Gets the mouse world position
        /// </summary>
        /// <returns>The vec</returns>
        public static Vector3 GetMouseWorldPosition()
        {
            var vec = GetMouseWorldPositionWithZ(Mouse.current.position.ReadValue(), Camera.main);
            vec.z = 0f;
            return vec;
        }
    
        /// <summary>
        /// Gets the mouse world position with z using the specified screen position
        /// </summary>
        /// <param name="screenPosition">The screen position</param>
        /// <param name="worldCamera">The world camera</param>
        /// <returns>The world position</returns>
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) 
        {
            var worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}
