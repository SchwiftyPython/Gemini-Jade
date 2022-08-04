using System;
using System.Collections.Generic;
using System.Linq;
using UI.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using World.Things.CraftableThings;

namespace World
{
    public class GridBuildingSystem : MonoBehaviour
    {
        private static readonly Dictionary<int, int> BitMaskValueToIndex = new()
        {
            {0, 0},
            {2, 1},
            {16, 2},
            {64, 3},
            {8, 4},
            {66, 5},
            {24, 6},
            {90, 7},
            {10, 8},
            {18, 9},
            {80, 10},
            {72, 11},
            {11, 12},
            {22, 13},
            {208, 14},
            {31, 15},
            {214, 16},
            {248, 17},
            {107, 18},
            {95, 19},
            {123, 20},
            {250, 21},
            {222, 22},
            {26, 23},
            {82, 24},
            {88, 25},
            {74, 26},
            {223, 27},
            {254, 28},
            {251, 29},
            {127, 30},
            {216, 31},
            {120, 32},
            {27, 33},
            {30, 34},
            {219, 35},
            {94, 36},
            {126, 37},
            {218, 38},
            {91, 39},
            {122, 40},
            {255, 41},
            {210, 42},
            {86, 43},
            {75, 44},
            {106, 45},
            {104, 46}
        };

        public static int CalculateTileIndex(bool east, bool west, bool north, bool south, bool northWest,
            bool northEast, bool southWest, bool southEast)
        {
            var direction = (east ? BitMaskDirection.East : 0) | (west ? BitMaskDirection.West : 0)  | (north ? BitMaskDirection.North : 0) | (south ? BitMaskDirection.South : 0);
            
            direction |= north && west && northWest ? BitMaskDirection.NorthWest : 0;
            
            direction |= north && east && northEast ? BitMaskDirection.NorthEast : 0;
            
            direction |= south && west && southWest ? BitMaskDirection.SouthWest : 0;
            
            direction |= south && east && southEast ? BitMaskDirection.SouthEast : 0;

            return BitMaskValueToIndex[(int) direction];
        }
        
        public event EventHandler OnSelectedChanged;

        public LocalMap LocalMap { get; private set; }

        private PlacedObjectTemplate _selectedObjectType;
        
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
                    if (!LocalMap.CanPlaceGridObjectAt(gridPosition.ToCoord()))
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    ghostObject.ColorSpriteWhite();
                    
                    if (Mouse.current.leftButton.isPressed)
                    {
                        PlacedObject placedObject;
                        
                        if (_selectedObjectType.isWall)
                        {
                            placedObject = WallPlacedObject.Create(mousePosition.ToVector2Int(), _dir,
                                _selectedObjectType);
                        }
                        else
                        {
                            placedObject = PlacedObject.Create(mousePosition.ToVector2Int(), _dir,
                                _selectedObjectType);
                        }

                        LocalMap.PlacePlacedObject(placedObject);
                    }
                }
                else
                {
                    ghostObject.ColorSpriteRed();
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
            LocalMap = localMap;
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
                
                case PlacedObject.Dir.Left: return new Vector2Int(1, 0);
                
                case PlacedObject.Dir.Up: return new Vector2Int(1,1);
                
                case PlacedObject.Dir.Right: return new Vector2Int(0, 1);
            }
        }

        private void DeselectObjectType()
        {
            _selectedObjectType = null;
            
            ghostObject.Hide();
        }

        private void OnObjectSelected(PlacedObjectTemplate objectType)
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
