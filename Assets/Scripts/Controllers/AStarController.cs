using UnityEngine;
using World;

namespace Controllers
{
    /// <summary>
    /// Makes sure <see cref="AstarPath"/> is matched up with current <see cref="LocalMap"/>'s dimensions.
    /// </summary>
    public class AStarController : MonoBehaviour
    {
        private void Awake()
        {
            var game = FindObjectOfType<Game>();

            game.onNewLocalMap += UpdatePathfinder;
        }

        /// <summary>
        /// Sets dimensions of Grid Graph to match Local Map
        /// </summary>
        /// <param name="localMap"></param>
        private static void UpdatePathfinder(LocalMap localMap)
        {
            AstarPath.active.data.gridGraph.SetDimensions(localMap.Width, localMap.Height, 1);

            AstarPath.active.data.gridGraph.center = localMap.GetMapCenterWithOffset();
            
            AstarPath.active.Scan();
        }
    }
}
