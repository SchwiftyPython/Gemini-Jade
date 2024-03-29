using System;
using GoRogue;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace World.Pawns
{
    /// <summary>
    /// The pawn movement class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class PawnMovement : MonoBehaviour
    {
        /// <summary>
        /// The next waypoint distance
        /// </summary>
        private const float NextWaypointDistance = .02f;

        /// <summary>
        /// The repath rate
        /// </summary>
        private const float RepathRate = 0.5f;
        
        /// <summary>
        /// The seeker
        /// </summary>
        private Seeker _seeker;
        
        /// <summary>
        /// The pawn
        /// </summary>
        private Pawn _pawn;

        /// <summary>
        /// The reached end of path
        /// </summary>
        private bool _reachedEndOfPath;

        /// <summary>
        /// The speed
        /// </summary>
        private float _speed;

        /// <summary>
        /// The current waypoint
        /// </summary>
        private int _currentWaypoint;
        
        /// <summary>
        /// The negative infinity
        /// </summary>
        private float _lastRepath = float.NegativeInfinity;

        /// <summary>
        /// The on change direction
        /// </summary>
        private Action<Direction> _onChangeDirection;
        
        /// <summary>
        /// The on destination reached
        /// </summary>
        public Action onDestinationReached;
        
        /// <summary>
        /// The on destination unreachable
        /// </summary>
        public Action onDestinationUnreachable;

        /// <summary>
        /// Gets or sets the value of the destination
        /// </summary>
        private Coord Destination { get; set; }

        /// <summary>
        /// Gets or sets the value of the facing
        /// </summary>
        private Direction Facing { get; set; }

        /// <summary>
        /// Gets or sets the value of the path
        /// </summary>
        private Path Path { get; set; }

        /// <summary>
        /// Gets the value of the has destination
        /// </summary>
        private bool HasDestination => Path != null;

        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start () 
        {
            _seeker = GetComponent<Seeker>();
            
            _seeker.pathCallback += OnPathComplete;
        }
        
        /// <summary>
        /// Updates this instance
        /// </summary>
        private void Update () 
        {
            if (!HasDestination)
            {
                return;
            }

            _reachedEndOfPath = false;

            while (!_reachedEndOfPath)
            {
                var distanceToWaypoint = Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]);

                if (distanceToWaypoint < NextWaypointDistance)
                {
                    if(_currentWaypoint + 1 < Path.vectorPath.Count)
                    {
                        _currentWaypoint++;
                    }
                    else
                    {
                        _reachedEndOfPath = true;
                        
                        onDestinationReached?.Invoke();
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            
            var waypoint = Path.vectorPath[_currentWaypoint];

            if (!((LocalMap) _pawn.CurrentMap).WalkableAt(waypoint.ToCoord()))
            {
                Path = null;
                
                _reachedEndOfPath = true;

                //todo if this is null maybe consider clearing goals as a fail-safe
                onDestinationUnreachable?.Invoke();

                return;
            }
            
            var speedFactor = 1f; //todo calculate speed factor based on terrain movement speed and pawn movement function

            var calculatedPosition = transform.position;

            var dir = (Path.vectorPath[_currentWaypoint] - calculatedPosition).normalized;
            
            var velocity = dir * (_speed * speedFactor);

            calculatedPosition += velocity * UnityEngine.Time.deltaTime;
            
            UpdateFacing(calculatedPosition.ToCoord());

            transform.position = calculatedPosition;
            
            _pawn.Position = calculatedPosition.ToCoord();
        }

        /// <summary>
        /// Inits the pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public void Init(Pawn pawn)
        {
            _pawn = pawn;

            _speed = pawn.species.baseSpeed;

            _onChangeDirection += pawn.UpdateSpriteFacing;
            
            Reset();
        }

        /// <summary>
        /// Describes whether this instance can path to
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <returns>The bool</returns>
        public bool CanPathTo(Coord destination)
        {
            var path = _seeker.StartPath(_pawn.Position.ToVector3(), destination.ToVector3());
            
            path.BlockUntilCalculated();

            return !path.error && path.vectorPath.Count > 1;
        }

        /// <summary>
        /// Moves the to using the specified destination
        /// </summary>
        /// <param name="destination">The destination</param>
        public void MoveTo(Coord destination)
        {
            if (!((LocalMap) _pawn.CurrentMap).WalkableAt(destination))
            {
                _reachedEndOfPath = true;

                onDestinationUnreachable?.Invoke();
                
                return;
            }

            if (UnityEngine.Time.time <= _lastRepath + RepathRate)
            {
                return;
            }

            if (_seeker.IsDone())
            {
                _lastRepath = UnityEngine.Time.time;
            }
                
            Destination = destination;
            
            _seeker.StartPath(_pawn.Position.ToVector3(), Destination.ToVector3());
        }

        /// <summary>
        /// Ons the path complete using the specified path
        /// </summary>
        /// <param name="path">The path</param>
        private void OnPathComplete (Path path) 
        {
            if (!path.error && path.vectorPath.Count > 0)
            {
                Path = path;

                _currentWaypoint = 0;
            }
            else if(path.error)
            {
                onDestinationUnreachable?.Invoke();
                
                Debug.LogError($"Path error for pawn id {_pawn.id}: {path.error}");
            }
            else
            {
                onDestinationUnreachable?.Invoke();
                
                Debug.Log($"Destination {Destination} unreachable for pawn id {_pawn.id}");
            }
        }

        /// <summary>
        /// Updates the facing using the specified target
        /// </summary>
        /// <param name="target">The target</param>
        public void UpdateFacing(Coord target)
        {
            var currentFacing = Facing;
            
            var delta = target - _pawn.Position;

            var deltaXAbs = Math.Abs(delta.X);
            
            var deltaYAbs = Math.Abs(delta.Y);

            if (deltaXAbs > deltaYAbs)
            {
                if (delta.X > 0)
                {
                    Facing = Direction.RIGHT;
                }
                else if (delta.X < 0)
                {
                    Facing = Direction.LEFT;
                }
            }
            else
            {
                if (delta.Y > 0)
                {
                    Facing = Direction.UP;
                }
                else if (delta.Y < 0)
                {
                    Facing = Direction.DOWN;
                }
            }

            if (currentFacing == Facing)
            {
                return;
            }

            _onChangeDirection?.Invoke(Facing);
        }

        /// <summary>
        /// Resets this instance
        /// </summary>
        private void Reset()
        {
            Destination = _pawn.Position;

            Path = null;
        }
        
        /// <summary>
        /// Ons the disable
        /// </summary>
        private void OnDisable () 
        {
            _seeker.pathCallback -= OnPathComplete;
        }
    }
}
