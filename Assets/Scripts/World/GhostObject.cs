using UI.Grid;
using UnityEngine;
using World.PlacedObjectTypes;

namespace World
{
    public class GhostObject : MonoBehaviour
    {
        private Transform _instance;
    
        private PlacedObjectType _placedObjectType;
        
        private GridBuildingSystem _gridBuildingSystem;

        private void Start()
        {
            _gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
            
            SelectGridObjectButton.OnObjectSelected += OnObjectSelected;
        }


        private void LateUpdate()
        {
            //todo object follows mouse and snaps to grid

            var targetPosition = GridBuildingSystem.GetMouseWorldSnappedPosition();
        
            transform.position = Vector3.Lerp(transform.position, targetPosition, UnityEngine.Time.deltaTime * 15f);
            
            transform.rotation = Quaternion.Lerp(transform.rotation, _gridBuildingSystem.GetObjectRotation(), UnityEngine.Time.deltaTime * 15f);
        }

        private void OnObjectSelected(PlacedObjectType objectType)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);

                _instance = null;
            }

            if (objectType != null)
            {
                _placedObjectType = objectType;
            
                _instance = Instantiate(_placedObjectType.prefab, Vector3.zero, Quaternion.identity, transform);
            
                _instance.localPosition = Vector3.zero;
            
                _instance.localEulerAngles = Vector3.zero;
            }
        }
    }
}
