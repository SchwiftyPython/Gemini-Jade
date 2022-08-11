using GoRogue;
using Pathfinding;
using UnityEngine;
using Utilities;

namespace World.Pawns
{
    public class PawnMovement : MonoBehaviour
    {
        private const float NextWaypointDistance = 0.5f;
        
        private Seeker _seeker;
        
        private Pawn _pawn;

        private bool _reachedEndOfPath;

        private float _speed;

        private int _currentWaypoint;
        
        public System.Action<Direction> onChangeDirection;
        
        public System.Action onDestinationReached;

        public Coord Position { get; protected set; }

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
        
        private void Update () 
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
            
            var speedFactor = 1f; //todo calculate speed factor based on terrain movement speed

            var position = transform.position;
            
            //todo doesn't quite follow on the path sometimes and also passes over walls
            //which leads me to believe it's offset from the path a bit after these calculations
            //maye do a debug.log comparison of the path position and the position assigned ot the pawn
            
            var dir = (Path.vectorPath[_currentWaypoint] - position).normalized;
            
            var velocity = dir * (_speed * speedFactor);
            
            UpdateFacing(Path.vectorPath[_currentWaypoint].ToCoord());
            
            position += velocity * UnityEngine.Time.deltaTime;
            
            transform.position = position;
            
            Position = position.ToCoord();
            
            _pawn.Position = Position;
        }

        public void Init(Pawn pawn)
        {
            _pawn = pawn;
            
            Position = pawn.Position;
            
            _speed = pawn.species.baseSpeed;

            onChangeDirection += pawn.UpdateSpriteFacing;
            
            Reset();
        }
        
        public void MoveTo(Coord destination)
        {
            Destination = destination;
            
            _seeker.StartPath(Position.ToVector3(), Destination.ToVector3());
        }

        private void OnPathComplete (Path path) 
        {
            if (!path.error)
            {
                Path = path;

                _currentWaypoint = 0;
            }
            else
            {
                Debug.LogError($"Path error for pawn id {_pawn.id}: {path.error}");
            }
        }

        private void UpdateFacing(Coord target)
        {
            var currentFacing = Facing;
            
            var delta = target - Position;

            if (delta.X > 0)
            {
                Facing = Direction.RIGHT;
            }
            else if (delta.X < 0)
            {
                Facing = Direction.LEFT;
            }
            else if (delta.Y > 0)
            {
                Facing = Direction.UP;
            }
            else if (delta.Y < 0)
            {
                Facing = Direction.DOWN;
            }

            if (currentFacing == Facing)
            {
                return;
            }

            onChangeDirection?.Invoke(Facing);
        }

        private void Reset()
        {
            Destination = Position;

            Path = null;
        }
        
        private void OnDisable () 
        {
            _seeker.pathCallback -= OnPathComplete;
        }
    }
}
