using System;
using UI.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using World.PlacedObjectTypes;

namespace World
{
    public class GridBuildingSystem : MonoBehaviour
    {
        public event EventHandler OnSelectedChanged;
    
        private LocalMap _localMap;
    
        private PlacedObjectType _selectedObjectType;
        
        [SerializeField] private GhostObject ghostObject;

        private PlacedObject.Dir _dir;

        private bool _placingObject;

        private void Awake()
        {
            SelectGridObjectButton.OnObjectSelected += OnObjectSelected;
        }
        
        private void Update()
        {
            if (_placingObject)
            {
                var mousePosition = GetMouseGridSnappedPosition();

                var gridPositions = ghostObject.GetGridPositions(new Vector2Int((int) mousePosition.x, (int) mousePosition.y), _dir);

                var canPlace = true;
                
                foreach (var gridPosition in gridPositions)
                {
                    if (!_localMap.CanPlaceGridObjectAt(gridPosition.ToCoord()))
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    if (Mouse.current.leftButton.isPressed)
                    {
                        var placedObject = PlacedObject.Create(new Vector3(mousePosition.x, mousePosition.y), _dir,
                            _selectedObjectType);
                    
                        foreach (var gridPosition in gridPositions)
                        {
                            _localMap.PlaceGridObjectAt(gridPosition.ToCoord(), placedObject.GridObject);
                        }
                    }
                }

                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    _dir = GetNextDir();
                }

                if(Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    DeselectObjectType();
                    
                    _placingObject = false;
                }
            }
        }
        
        public Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = MouseUtils.GetMouseWorldPosition();
            
            var rotationOffset = GetRotationOffset();
            
            var offsetPosition = mousePosition + new Vector3(rotationOffset.x, rotationOffset.y, 0f);

            var snappedPosition = new Vector3((int) offsetPosition.x, (int) offsetPosition.y);

            return snappedPosition;

        }

        public Vector3 GetMouseGridSnappedPosition()
        {
            var mousePosition = MouseUtils.GetMouseWorldPosition();
            
            var snappedPosition = new Vector3((int) mousePosition.x, (int) mousePosition.y);

            return snappedPosition;
        }
        
        public void SetLocalMap(LocalMap localMap)
        {
            _localMap = localMap;
        }
        
        public Quaternion GetObjectRotation()
        {
            int rotationAngle;
            
            switch (_dir)
            {
                default:
                case PlacedObject.Dir.Down: rotationAngle = 0;
                    break;

                case PlacedObject.Dir.Left: rotationAngle = 90;
                    break;

                case PlacedObject.Dir.Up: rotationAngle = 180;
                    break;

                case PlacedObject.Dir.Right: rotationAngle = 270;
                    break;
            }
            
            return Quaternion.Euler(0, 0, rotationAngle);
        }
        
        public PlacedObject.Dir GetNextDir()
        {
            switch (_dir)
            {
                default:
                case PlacedObject.Dir.Down: return PlacedObject.Dir.Left;
                case PlacedObject.Dir.Left: return PlacedObject.Dir.Up;
                case PlacedObject.Dir.Up: return PlacedObject.Dir.Right;
                case PlacedObject.Dir.Right: return PlacedObject.Dir.Down;
            }
        }
        
        public Vector2Int GetRotationOffset()
        {
            switch (_dir)
            {
                default:
                case PlacedObject.Dir.Down: return new Vector2Int(0, 0);
                
                case PlacedObject.Dir.Left: return new Vector2Int(_selectedObjectType.width, 0);
                
                case PlacedObject.Dir.Up: return new Vector2Int(_selectedObjectType.height, _selectedObjectType.width);
                
                case PlacedObject.Dir.Right: return new Vector2Int(0, _selectedObjectType.height);
            }
        }

        private void DeselectObjectType()
        {
            _selectedObjectType = null;
            
            ghostObject.Hide();
        }

        private void OnObjectSelected(PlacedObjectType objectType)
        {
            if (objectType == null)
            {
                return;
            }
            
            _dir = PlacedObject.Dir.Down;
            
            _selectedObjectType = objectType;
            
            ghostObject.Setup(objectType);

            _placingObject = true;
        }
    }
}
