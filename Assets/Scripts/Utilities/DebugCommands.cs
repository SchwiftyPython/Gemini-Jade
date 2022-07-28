using UnityEngine;
using World;

namespace Utilities
{
    public class DebugCommands : MonoBehaviour
    {
        public void FinishAllBlueprints()
        {
            var blueprints = FindObjectOfType<GridBuildingSystem>().LocalMap.GetAllBlueprints();

            foreach (var blueprint in blueprints)
            {
                blueprint.Make();
            }
        }
    }
}
