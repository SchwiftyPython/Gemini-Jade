using System;
using GoRogue;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace World.Pawns
{
    public class PawnMovement : MonoBehaviour
    {
        private const int MaxFailures = 5;
        
        private const float NextWaypointDistance = .02f;

        private const float RepathRate = 0.5f;
        
        private Seeker _seeker;
        
        private Pawn _pawn;

        private bool _reachedEndOfPath;

        private float _speed;

        private int _currentWaypoint;
        
        private float _lastRepath = float.NegativeInfinity;

        private int _numFailures;
        
        public Action<Direction> onChangeDirection;
        
        public Action onDestinationReached;
        
        public Action onDestinationUnreachable;

        public Coord Destination { get; private set; }

        public Direction Facing { get; private set; }

        public Path Path { get; private set; }

        public bool HasDestination => Path != null;
        
        public bool ReachedDestination => _reachedEndOfPath;

        private void Start () 
        {
            _seeker = GetComponent<Seeker>();
            
            _seeker.pathCallback += OnPathComplete;
        }
        
        private void LateUpdate () 
        {
            if (!HasDestination)
            {
                return;
            }

            _reachedEndOfPath = false;

            float distanceToWaypoint;
            
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]);

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
                
                Debug.Log($"Path invalid. Next waypoint not walkable in path {waypoint}");
                
                onDestinationUnreachable?.Invoke();

                return;
            }
            
            var speedFactor = 1f; //todo calculate speed factor based on terrain movement speed

            var calculatedPosition = transform.position;

            var dir = (Path.vectorPath[_currentWaypoint] - calculatedPosition).normalized;
            
            var velocity = dir * (_speed * speedFactor);

            calculatedPosition += velocity * UnityEngine.Time.deltaTime;
            
            UpdateFacing(calculatedPosition.ToCoord());

            transform.position = calculatedPosition;
            
            _pawn.Position = calculatedPosition.ToCoord();
        }

        public void Init(Pawn pawn)
        {
            _pawn = pawn;

            _speed = pawn.species.baseSpeed;

            onChangeDirection += pawn.UpdateSpriteFacing;
            
            Reset();
        }
        
        public void MoveTo(Coord destination)
        {
            if (!((LocalMap) _pawn.CurrentMap).WalkableAt(destination))
            {
                _reachedEndOfPath = true;
                
                Debug.Log($"Chosen Destination {destination} is not walkable");
                
                onDestinationUnreachable?.Invoke();
                
                return;
            }
            
            if (UnityEngine.Time.time > _lastRepath + RepathRate)
            {
                if (_seeker.IsDone())
                {
                    _lastRepath = UnityEngine.Time.time;
                }
            }
            
            Destination = destination;
            
            _seeker.StartPath(_pawn.Position.ToVector3(), Destination.ToVector3());
        }

        private void OnPathComplete (Path path) 
        {
            if (!path.error && path.vectorPath.Count > 1)
            {
                Path = path;

                _currentWaypoint = 0;

                _numFailures = 0;
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

        private void UpdateFacing(Coord target)
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

            onChangeDirection?.Invoke(Facing);
        }

        private void Reset()
        {
            Destination = _pawn.Position;

            Path = null;
        }
        
        private void OnDisable () 
        {
            _seeker.pathCallback -= OnPathComplete;
        }
    }
}
