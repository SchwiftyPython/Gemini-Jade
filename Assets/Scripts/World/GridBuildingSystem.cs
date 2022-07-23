using System;
using UI.Grid;
using UnityEngine;
using Utilities;
using World.PlacedObjectTypes;

namespace World
{
    public class GridBuildingSystem : MonoBehaviour
    {
        public static Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = MouseUtils.GetMouseWorldPosition();

            var snappedPosition = new Vector3((int) mousePosition.x, (int) mousePosition.y); //hail mary

            return snappedPosition;
        }

        public event EventHandler OnSelectedChanged;
    
        private LocalMap _localMap;
    
        private PlacedObjectType _selectedObjectType;

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
                //todo check if can build
                
                //todo react to left mouse click to try place
                
                //todo react to r key pressed for rotation
                
                //todo react to esc or rmb to cancel
            }
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
            
            return Quaternion.Euler(0, rotationAngle, 0);
        }

        private void OnObjectSelected(PlacedObjectType objectType)
        {
            if (objectType == null)
            {
                return;
            }
            
            _dir = PlacedObject.Dir.Down;
            
            _selectedObjectType = objectType;

            _placingObject = true;
        }
    }
}
