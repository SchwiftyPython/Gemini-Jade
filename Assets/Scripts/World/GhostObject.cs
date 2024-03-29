using System.Collections.Generic;
using UnityEngine;
using World.Things.CraftableThings;

namespace World
{
    /// <summary>
    /// The ghost object class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class GhostObject : MonoBehaviour
    {
        /// <summary>
        /// The ghost object layer name
        /// </summary>
        private const string GhostObjectLayerName = "GhostObject";
        
        /// <summary>
        /// The instance
        /// </summary>
        private Transform _instance;

        /// <summary>
        /// The placed object ghost
        /// </summary>
        private PlacedObject _placedObjectGhost;
    
        /// <summary>
        /// The placed object type
        /// </summary>
        private PlacedObjectTemplate _placedObjectType;
        
        /// <summary>
        /// The grid building system
        /// </summary>
        private GridBuildingSystem _gridBuildingSystem;

        /// <summary>
        /// Awakes this instance
        /// </summary>
        private void Awake()
        {
            _gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
            
            Hide();
        }


        /// <summary>
        /// Lates the update
        /// </summary>
        private void LateUpdate()
        {
            var targetPosition = _gridBuildingSystem.GetMouseWorldSnappedPosition();

            transform.position = Vector3.Lerp(transform.position, targetPosition, UnityEngine.Time.deltaTime * 25f);

            transform.rotation = Quaternion.Lerp(transform.rotation, _gridBuildingSystem.GetObjectRotation(), UnityEngine.Time.deltaTime * 25f);
        }

        /// <summary>
        /// Shows this instance
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hides this instance
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Setup the object type
        /// </summary>
        /// <param name="objectType">The object type</param>
        public void Setup(PlacedObjectTemplate objectType)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);

                _instance = null;
            }

            if (objectType == null)
            {
                return;
            }

            _placedObjectType = objectType;
            
            _instance = Instantiate(_placedObjectType.Prefab, Vector3.zero, Quaternion.identity, transform);
            
            _instance.localPosition = Vector3.zero;
            
            _instance.localEulerAngles = Vector3.zero;
                
            _placedObjectGhost = _instance.GetComponent<PlacedObject>();
                
            _placedObjectGhost.SpriteRenderer.sprite = NeedsToBeMade
                ? objectType.blueprintTexture
                : objectType.builtTexture;

            _placedObjectGhost.SpriteRenderer.sortingLayerName = GhostObjectLayerName;
                
            Show();
        }
        
        /// <summary>
        /// Gets the value of the needs to be made
        /// </summary>
        public bool NeedsToBeMade => _placedObjectType.workToMake > 0;

        /// <summary>
        /// Colors the sprite red
        /// </summary>
        public void ColorSpriteRed()
        {
            _placedObjectGhost.SpriteRenderer.color = Color.red;
        }

        /// <summary>
        /// Colors the sprite white
        /// </summary>
        public void ColorSpriteWhite()
        {
            _placedObjectGhost.SpriteRenderer.color = Color.white;
        }
        
        /// <summary>
        /// Gets the grid positions using the specified origin
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="dir">The dir</param>
        /// <returns>The grid position list</returns>
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

        /// <summary>
        /// Ons the object selected using the specified object type
        /// </summary>
        /// <param name="objectType">The object type</param>
        private void OnObjectSelected(PlacedObjectTemplate objectType)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);

                _instance = null;
            }

            if (objectType != null)
            {
                _placedObjectType = objectType;
            
                _instance = Instantiate(_placedObjectType.Prefab, Vector3.zero, Quaternion.identity, transform);
            
                _instance.localPosition = Vector3.zero;
            
                _instance.localEulerAngles = Vector3.zero;
            }
        }
    }
}
