using System.Collections.Generic;
using UnityEngine;
using World.PlacedObjectTypes;

namespace World
{
    public class GhostObject : MonoBehaviour
    {
        private const string GhostObjectLayerName = "GhostObject";
        
        private Transform _instance;
    
        private PlacedObjectType _placedObjectType;
        
        private GridBuildingSystem _gridBuildingSystem;

        private void Awake()
        {
            _gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
            
            Hide();
        }


        private void LateUpdate()
        {
            var targetPosition = _gridBuildingSystem.GetMouseWorldSnappedPosition();

            transform.position = Vector3.Lerp(transform.position, targetPosition, UnityEngine.Time.deltaTime * 25f);

            transform.rotation = Quaternion.Lerp(transform.rotation, _gridBuildingSystem.GetObjectRotation(), UnityEngine.Time.deltaTime * 25f);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
            
            transform.position = Vector3.zero;
        }

        public void Setup(PlacedObjectType objectType)
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
                
                var placedObjectGhost = _instance.GetComponent<PlacedObject>();
                
                //todo need to check if it has a blueprint. If not skip the blueprint texture

                placedObjectGhost.SpriteRenderer.sprite = objectType.blueprintTexture;

                placedObjectGhost.SpriteRenderer.sortingLayerName = GhostObjectLayerName;
                
                Show();
            }
        }
        
        public List<Vector2Int> GetGridPositions(Vector2Int origin, PlacedObject.Dir dir)
        {
            var gridPositionList = new List<Vector2Int>();
            
            switch (dir)
            {
                default:
                case PlacedObject.Dir.Down:
                    for (var x = 0; x < _placedObjectType.width; x++)
                    {
                        for (var y = 0; y < _placedObjectType.height; y++)
                        {
                            gridPositionList.Add(origin + new Vector2Int(x, y));
                        }
                    }

                    break;
                
                case PlacedObject.Dir.Up:
                    for (var x = _placedObjectType.width - 1; x >= 0; x--)
                    {
                        for (var y = _placedObjectType.height - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(origin - new Vector2Int(x, y));
                        }
                    }

                    break;
                
                case PlacedObject.Dir.Left:
                    for (var x = 0; x < _placedObjectType.height; x++)
                    {
                        for (var y = 0; y < _placedObjectType.width; y++)
                        {
                            gridPositionList.Add(origin + new Vector2Int(x, y));
                        }
                    }

                    break;
                
                case PlacedObject.Dir.Right:
                    for (var x = _placedObjectType.height - 1; x >= 0; x--)
                    {
                        for (var y = _placedObjectType.width - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(origin - new Vector2Int(x, y));
                        }
                    }
                    
                    break;
            }

            return gridPositionList;
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
