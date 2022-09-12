using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UI.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
using World.Things.CraftableThings;

namespace World
{
    /// <summary>
    /// Handles placing and removing <see cref="PlacedObject"/>s on the current <see cref="LocalMap"/>
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class GridBuildingSystem : MonoBehaviour
    {
        /// <summary>
        /// Stores bitmask value to index conversions
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
        /// Calculates the tile index depending on what directions have neighbors.
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
        /// The ghost object prefab
        /// </summary>
        [SerializeField] private GhostObject ghostObjectPrefab;

        private List<GhostObject> ghostObjects;

        /// <summary>
        /// The current direction the object is facing or wall being built in
        /// </summary>
        private Direction _dir;

        /// <summary>
        /// Indicates if an object is currently selected and being placed
        /// </summary>
        private bool _placingObject;

        /// <summary>
        /// Indicates if currently placing walls
        /// </summary>
        private bool _placingWalls;

        /// <summary>
        /// Indicates if an origin has been picked for the wall and the user is determining length
        /// </summary>
        private bool _wallStarted;

        /// <summary>
        /// Stores the last ghost object placed
        /// </summary>
        private GhostObject _lastGhost;

        public Action onDraggingStarted;

        public Action onDraggingEnded;

        private void Awake()
        {
            SelectGridObjectButton.OnObjectSelected += OnObjectSelected;
        }

        private void Update()
        {
            if (_placingObject)
            {
                HandleSingleObjectPlacement();
            }
            else if (_placingWalls)
            {
                HandleWallPlacement();
            }
        }

        /// <summary>
        /// Handles input while placing walls
        /// </summary>
        private void HandleWallPlacement()
        {
            if (!_wallStarted)
            {
                var mousePosition = GetMouseGridSnappedPosition();

                if (LocalMap.CanPlaceGridObjectAt(mousePosition.ToCoord()))
                {
                    ghostObjects.First().ColorSpriteWhite();
                }
                else
                {
                    ghostObjects.First().ColorSpriteRed();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (_wallStarted)
                {
                    UpdateWall();
                    return;
                }

                StartWall();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!_wallStarted)
                {
                    return;
                }

                PlaceWall();
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelWall();
            }
        }

        /// <summary>
        /// Places <see cref="GhostObject"/> at current mouse position and allows player to adjust length of wall
        /// </summary>
        private void StartWall()
        {
            _wallStarted = true;
            
            onDraggingStarted?.Invoke();
            
            var startPosition = GetMouseGridSnappedPosition();

            ghostObjects.First().transform.position = startPosition;

            _lastGhost = ghostObjects.First();
        }

        /// <summary>
        /// Places all <see cref="GhostObject"/>s that are in valid positions.
        /// </summary>
        private void PlaceWall()
        {
            _wallStarted = false;
            
            onDraggingEnded?.Invoke();

            foreach (var ghost in ghostObjects)
            {
                if (!LocalMap.CanPlaceGridObjectAt(ghost.transform.position.ToCoord()))
                {
                    continue;
                }
                
                var placedObject = WallPlacedObject.Create(ghost.transform.position.ToVector2Int(), _dir,
                    _selectedObjectType);

                LocalMap.PlacePlacedObject(placedObject);
            }
            
            DestroyGhostObjects();

            OnObjectSelected(_selectedObjectType);
        }

        /// <summary>
        /// Adjusts length and <see cref="Direction"/> of wall once wall is started
        /// </summary>
        private void UpdateWall()
        {
            var mousePosition = GetMouseGridSnappedPosition();

            if (!mousePosition.Equals(_lastGhost.transform.position))
            {
                var coordDelta = mousePosition.ToCoord() - _lastGhost.transform.position.ToCoord();
                
                var currentDirection =
                    Direction.GetDirection(coordDelta);

                if (ghostObjects.Count == 1)
                {
                    if (currentDirection == Direction.UP || currentDirection == Direction.DOWN ||
                        currentDirection == Direction.LEFT || currentDirection == Direction.RIGHT)
                    {
                        _dir = currentDirection;
                    }
                    else
                    {
                        _dir = Direction.GetCardinalDirection(coordDelta);
                    }
                }
                
                if (_dir != currentDirection)
                {
                    HandleWallDirectionChange(currentDirection);
                }
                else
                {
                    var ghostPosition = _lastGhost.transform.position.ToCoord() + _dir;
                    
                    AddGhostObject(ghostPosition);
                }
            }
        }

        /// <summary>
        /// Changes direction of wall if current direction is perpendicular or opposite of wall direction
        /// </summary>
        /// <param name="currentDirection"></param>
        private void HandleWallDirectionChange(Direction currentDirection)
        {
            if (DirectionPerpendicularTo(currentDirection))
            {
                var prevGhost = ghostObjects.First();

                foreach (var ghost in ghostObjects)
                {
                    if (ghost == ghostObjects.First())
                    {
                        continue;
                    }

                    ghost.transform.position = (prevGhost.transform.position.ToCoord() + currentDirection).ToVector3();
                }

                _dir = currentDirection;
            }
            else if(DirectionIsOppositeOf(currentDirection))
            {
                if (ghostObjects.Count == 1)
                {
                    return;
                }

                var ghostToRemove = ghostObjects.Last();

                ghostObjects.Remove(ghostToRemove);
                        
                Destroy(ghostToRemove.gameObject);

                _lastGhost = ghostObjects.Last();
            }
        }

        /// <summary>
        /// Adds a <see cref="GhostObject"/> at the specified position
        /// </summary>
        /// <param name="position"></param>
        private void AddGhostObject(Coord position)
        {
            var newGhost = Instantiate(ghostObjectPrefab, position.ToVector3(), Quaternion.identity);

            newGhost.Setup(_selectedObjectType, false);

            newGhost.transform.position = position.ToVector3();
                
            ghostObjects.Add(newGhost);
                    
            if (!LocalMap.CanPlaceGridObjectAt(position))
            {
                newGhost.ColorSpriteRed();
            }

            _lastGhost = newGhost;
        }

        /// <summary>
        /// Determines if direction is perpendicular to wall direction
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>True if direction is perpendicular to wall direction</returns>
        private bool DirectionPerpendicularTo(Direction dir)
        {
            if (_dir == Direction.UP)
            {
                return dir == Direction.LEFT || dir == Direction.RIGHT;
            }

            if (_dir == Direction.DOWN)
            {
                return dir == Direction.LEFT || dir == Direction.RIGHT;
            }

            if (_dir == Direction.LEFT)
            {
                return dir == Direction.UP || dir == Direction.DOWN;
            }

            if (_dir == Direction.RIGHT)
            {
                return dir == Direction.UP || dir == Direction.DOWN;
            }

            return false;
        }

        /// <summary>
        /// Determines if direction is opposite of wall direction
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>True if direction is opposite of wall direction</returns>
        private bool DirectionIsOppositeOf(Direction dir)
        {
            if (_dir == Direction.UP)
            {
                return dir == Direction.DOWN;
            }

            if (_dir == Direction.DOWN)
            {
                return dir == Direction.UP;
            }

            if (_dir == Direction.LEFT)
            {
                return dir == Direction.RIGHT;
            }

            if (_dir == Direction.RIGHT)
            {
                return dir == Direction.LEFT;
            }

            return false;
        }

        /// <summary>
        /// Cancels current wall being placed
        /// </summary>
        private void CancelWall()
        {
            _wallStarted = false;

            _placingWalls = false;
            
            DeselectObjectType();

            onDraggingEnded?.Invoke();
        }

        /// <summary>
        /// Handles placing a single <see cref="PlacedObject"/>
        /// </summary>
        private void HandleSingleObjectPlacement()
        {
            var mousePosition = GetMouseGridSnappedPosition();

            var gridPositions = ghostObjects.First().GetGridPositions(new Vector2Int((int) mousePosition.x, (int) mousePosition.y), _dir);

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
                ghostObjects.First().ColorSpriteWhite();
                    
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
                ghostObjects.First().ColorSpriteRed();
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
            
            switch (_dir.Type)
            {
                default:
                case Direction.Types.DOWN: rotationAngle = 0;
                    break;

                case Direction.Types.LEFT: rotationAngle = 90;
                    break;

                case Direction.Types.UP: rotationAngle = 180;
                    break;

                case Direction.Types.RIGHT: rotationAngle = 270;
                    break;
            }
            
            return Quaternion.Euler(0, 0, rotationAngle);
        }

        /// <summary>
        /// Gets the next dir in clockwise order
        /// </summary>
        /// <returns>The placed object dir</returns>
        private Direction GetNextDir()
        {
            switch (_dir.Type)
            {
                default:
                case Direction.Types.DOWN: return Direction.LEFT;
                case Direction.Types.LEFT: return Direction.UP;
                case Direction.Types.UP: return Direction.RIGHT;
                case Direction.Types.RIGHT: return Direction.DOWN;
            }
        }

        /// <summary>
        /// Gets the rotation offset
        /// </summary>
        /// <returns>The vector int</returns>
        private Vector2Int GetRotationOffset()
        {
            switch (_dir.Type)
            {
                default:
                case Direction.Types.DOWN: return new Vector2Int(0, 0);
                
                case Direction.Types.LEFT: return new Vector2Int(1, 0);
                
                case Direction.Types.UP: return new Vector2Int(1,1);
                
                case Direction.Types.RIGHT: return new Vector2Int(0, 1);
            }
        }

        /// <summary>
        /// Deselects the object type
        /// </summary>
        private void DeselectObjectType()
        {
            _selectedObjectType = null;
            
            DestroyGhostObjects();
        }

        /// <summary>
        /// Sets up a <see cref="GhostObject"/> for the selected <see cref="PlacedObjectTemplate"/>
        /// </summary>
        /// <param name="objectType">The object type</param>
        private void OnObjectSelected(PlacedObjectTemplate objectType)
        {
            if (objectType == null)
            {
                return;
            }
            
            _dir = Direction.DOWN;
            
            _selectedObjectType = objectType;

            ghostObjects = new List<GhostObject>();

            var ghostObject = Instantiate(ghostObjectPrefab, GetMouseWorldSnappedPosition(), Quaternion.identity);
            
            ghostObjects.Add(ghostObject);
            
            ghostObject.Setup(objectType);

            if (_selectedObjectType.isWall)
            {
                _placingWalls = true;
            }
            else
            {
                _placingObject = true;
            }
        }

        /// <summary>
        /// Destroys all <see cref="GhostObject"/>s
        /// </summary>
        private void DestroyGhostObjects()
        {
            foreach (var ghostObject in ghostObjects)
            {
                Destroy(ghostObject.gameObject);
            }
        }
    }
}
