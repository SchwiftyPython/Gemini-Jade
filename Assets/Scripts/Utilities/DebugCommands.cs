using UnityEngine;
using World;

namespace Utilities
{
    /// <summary>
    /// The debug commands class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class DebugCommands : MonoBehaviour
    {
        /// <summary>
        /// Finishes the all blueprints
        /// </summary>
        public void FinishAllBlueprints()
        {
            var blueprints = FindObjectOfType<GridBuildingSystem>().LocalMap.GetAllBlueprints();

            foreach (var blueprint in blueprints)
            {
                if (blueprint.placedObjectType.isWall)
                {
                    ((WallPlacedObject)blueprint).FinishConstruction();
                }
                else
                {
                    blueprint.FinishConstruction();
                }
            }
        }
    }
}
