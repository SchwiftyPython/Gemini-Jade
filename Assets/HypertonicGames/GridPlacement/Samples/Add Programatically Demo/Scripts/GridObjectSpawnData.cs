using Hypertonic.GridPlacement.Enums;
using UnityEngine;

namespace Hypertonic.GridPlacement.Example.AddProgramatically.Models
{
    [System.Serializable]
    public class GridObjectSpawnData
    {
        public GameObject GridObject;
        public Vector2Int GridCellIndex;
        public ObjectAlignment ObjectAlignment;
        public Vector3 ObjectRotation;

        public GridObjectSpawnData(GameObject gridObject, Vector2Int gridCellIndex, ObjectAlignment objectAlignment, Vector3 objectRotation)
        {
            GridObject = gridObject;
            GridCellIndex = gridCellIndex;
            ObjectAlignment = objectAlignment;
            ObjectRotation = objectRotation;
        }
    }
}