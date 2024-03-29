using System.Collections.Generic;
using UI.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using World.Things.CraftableThings;

namespace World
{
    /// <summary>
    /// The grid building system class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class GridBuildingSystem : MonoBehaviour
    {
        /// <summary>
        /// The bit mask value to index
        /// </summary>
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
            {104, 15},
            {31, 16},
            {214, 17},
            {248, 18},
            {107, 19},
            {95, 20},
            {123, 21},
            {250, 22},
            {222, 23},
            {26, 24},
            {82, 25},
            {88, 26},
            {74, 27},
            {223, 28},
            {254, 29},
            {251, 30},
            {127, 31},
            {216, 32},
            {120, 33},
            {27, 34},
            {30, 35},
            {219, 36},
            {94, 37},
            {126, 38},
            {218, 39},
            {91, 40},
            {122, 41},
            {255, 42},
            {210, 43},
            {86, 44},
            {75, 45},
            {106, 46}
        };

        /// <summary>
        /// Calculates the tile index using the specified east
        /// </summary>
        /// <param name="east">The east</param>
        /// <param name="west">The west</param>
        /// <param name="north">The north</param>
        /// <param name="south">The south</param>
        /// <param name="northWest">The north west</param>
        /// <param name="northEast">The north east</param>
        /// <param name="southWest">The south west</param>
        /// <param name="southEast">The south east</param>
        /// <returns>The int</returns>
        public static int CalculateTileIndex(bool east, bool west, bool north, bool south, bool northWest,
            bool northEast, bool southWest, bool southEast)
        {
            var direction = (east ? BitMaskDirection.East : 0) | (west ? BitMaskDirection.West : 0) |
                            (north ? BitMaskDirection.North : 0) | (south ? BitMaskDirection.South : 0);

            direction |= north && west && northWest ? BitMaskDirection.NorthWest : 0;

            direction |= north && east && northEast ? BitMaskDirection.NorthEast : 0;

            direction |= south && west && southWest ? BitMaskDirection.SouthWest : 0;

            direction |= south && east && southEast ? BitMaskDirection.SouthEast : 0;

            return BitMaskValueToIndex[(int) direction];
        }

        /// <summary>
        /// Gets or sets the value of the local map
        /// </summary>
        public LocalMap LocalMap { get; private set; }

        /// <summary>
        /// The selected object type
        /// </summary>
        private PlacedObjectTemplate _selectedObjectType;
        
        /// <summary>
        /// The ghost object
        /// </summary>
        [SerializeField] private GhostObject ghostObject;

        /// <summary>
        /// The dir
        /// </summary>
        private PlacedObject.Dir _dir;

        /// <summary>
        /// The placing object
        /// </summary>
        private bool _placingObject;

        /// <summary>
        /// Awakes this instance
        /// </summary>
        private void Awake()
        {
            SelectGridObjectButton.OnObjectSelected += OnObjectSelected;
        }
        
        /// <summary>
        /// Updates this instance
        /// </summary>
        private void Update()
        {
            if (!_placingObject)
            {
                return;
            }

            var mousePosition = GetMouseGridSnappedPosition();

            var gridPositions = ghostObject.GetGridPositions(new Vector2Int((int) mousePosition.x, (int) mousePosition.y), _dir);

            var canPlace = true;

            foreach (var gridPosition in gridPositions)
            {
                if (LocalMap.CanPlaceGridObjectAt(gridPosition.ToCoord()))
                {
                    continue;
                }

                canPlace = false;
                break;
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
        
        /// <summary>
        /// Gets the mouse world snapped position
        /// </summary>
        /// <returns>The snapped position</returns>
        public Vector3 GetMouseWorldSnappedPosition()
        {
            var mousePosition = MouseUtils.GetMouseWorldPosition();
            
            var rotationOffset = GetRotationOffset();
            
            var offsetPosition = mousePosition + new Vector3(rotationOffset.x, rotationOffset.y, 0f);

            var snappedPosition = new Vector3((int) offsetPosition.x, (int) offsetPosition.y);

            return snappedPosition;

        }

        /// <summary>
        /// Gets the mouse grid snapped position
        /// </summary>
        /// <returns>The snapped position</returns>
        public Vector3 GetMouseGridSnappedPosition()
        {
            var mousePosition = MouseUtils.GetMouseWorldPosition();
            
            var snappedPosition = new Vector3((int) mousePosition.x, (int) mousePosition.y);

            return snappedPosition;
        }
        
        /// <summary>
        /// Sets the local map using the specified local map
        /// </summary>
        /// <param name="localMap">The local map</param>
        public void SetLocalMap(LocalMap localMap)
        {
            LocalMap = localMap;
        }
        
        /// <summary>
        /// Gets the object rotation
        /// </summary>
        /// <returns>The quaternion</returns>
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

        /// <summary>
        /// Gets the next dir
        /// </summary>
        /// <returns>The placed object dir</returns>
        private PlacedObject.Dir GetNextDir()
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

        /// <summary>
        /// Gets the rotation offset
        /// </summary>
        /// <returns>The vector int</returns>
        private Vector2Int GetRotationOffset()
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

        /// <summary>
        /// Deselects the object type
        /// </summary>
        private void DeselectObjectType()
        {
            _selectedObjectType = null;
            
            ghostObject.Hide();
        }

        /// <summary>
        /// Ons the object selected using the specified object type
        /// </summary>
        /// <param name="objectType">The object type</param>
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
