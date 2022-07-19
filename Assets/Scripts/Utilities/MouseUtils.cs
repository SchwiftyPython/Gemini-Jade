using UnityEngine;
using UnityEngine.InputSystem;

namespace Utilities
{
    public static class MouseUtils
    {
        public static Vector3 GetMouseWorldPosition()
        {
            var vec = GetMouseWorldPositionWithZ(Mouse.current.position.ReadValue(), Camera.main);
            vec.z = 0f;
            return vec;
        }
    
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) 
        {
            var worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}
