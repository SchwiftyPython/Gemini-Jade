using UnityEngine;
using World;

namespace Controllers
{
    /// <summary>
    /// Keeps camera up to date with current <see cref="LocalMap"/> dimensions
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject cameraBoundary;
        
        private void Awake()
        {
            var game = FindObjectOfType<Game>();

            game.onNewLocalMap += UpdateCameraBoundary;
        }

        /// <summary>
        /// Matches camera boundary's dimensions to Local Map
        /// </summary>
        /// <param name="localMap"></param>
        private void UpdateCameraBoundary(LocalMap localMap)
        {
            if (cameraBoundary == null)
            {
                cameraBoundary = GameObject.Find("CameraBoundary");
            }

            var boundaryCollider = cameraBoundary.GetComponent<BoxCollider>();

            boundaryCollider.size = new Vector3(localMap.Width, localMap.Height, boundaryCollider.size.z);

            cameraBoundary.transform.position = localMap.GetMapCenterWithOffset();
        }
    }
}
