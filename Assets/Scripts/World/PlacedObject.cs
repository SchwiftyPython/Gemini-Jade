using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UnityEngine;
using Utilities;
using World.Pawns;
using World.Pawns.Jobs;
using World.Things.CraftableThings;

namespace World
{
    /// <summary>
    /// The placed object class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class PlacedObject : MonoBehaviour
    {
        /// <summary>
        /// The blueprint color
        /// </summary>
        protected static readonly Color BlueprintColor = new(0.5f, 0.5f, 0.5f, 0.5f);

        /// <summary>
        /// The built color
        /// </summary>
        protected static readonly Color BuiltColor = new(1f, 1f, 1f, 1f);
        
        /// <summary>
        /// Creates the origin
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="direction">The direction</param>
        /// <param name="placedObjectType">The placed object type</param>
        /// <returns>The placed object</returns>
        public static PlacedObject Create(Vector2Int origin, Direction direction, PlacedObjectTemplate placedObjectType)
        {
            var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();

            var placedObjectInstance = Instantiate(placedObjectType.Prefab,
                gridBuildingSystem.GetMouseWorldSnappedPosition(), gridBuildingSystem.GetObjectRotation());

            var placedObject = placedObjectInstance.GetComponent<PlacedObject>();

            placedObject.instance = placedObjectInstance;
            
            placedObject.placedObjectType = placedObjectType;

            placedObject.map = gridBuildingSystem.LocalMap;

            MapLayer layer;
            bool walkable;
            bool transparent;

            placedObject.remainingWork = placedObjectType.workToMake;

            if (placedObject.NeedsToBeMade)
            {
                placedObject.SpriteRenderer.sprite = placedObjectType.blueprintTexture;
                
                placedObject.SpriteRenderer.color = BlueprintColor;

                walkable = true;
                
                transparent = true;
            }
            else
            {
                placedObject.spriteRenderer.sprite = placedObjectType.builtTexture;
                
                placedObject.SpriteRenderer.color = BuiltColor;

                walkable = placedObjectType.walkable;
                
                transparent = placedObjectType.transparent;
            }

            placedObject.direction = direction;

            placedObject.GridObjects = new List<GridObject>();

            placedObject.gridPositions = placedObject.GetGridPositions(origin, direction);

            foreach (var position in placedObject.gridPositions)
            {
                var gridObject = new GridObject(placedObject, position, true, walkable,
                    transparent);

                placedObject.GridObjects.Add(gridObject);
            }
            
            if (!walkable )
            {
                UnityUtils.AddBoxColliderTo(placedObjectInstance.gameObject);
            }

            return placedObject;
        }

        /// <summary>
        /// The dir enum
        /// </summary>
        public enum Dir 
        {
            /// <summary>
            /// The down dir
            /// </summary>
            Down,
            /// <summary>
            /// The left dir
            /// </summary>
            Left,
            /// <summary>
            /// The up dir
            /// </summary>
            Up,
            /// <summary>
            /// The right dir
            /// </summary>
            Right,
        }

        /// <summary>
        /// The instance
        /// </summary>
        protected Transform instance;
        
        /// <summary>
        /// The sprite renderer
        /// </summary>
        [SerializeField] protected internal SpriteRenderer spriteRenderer;

        /// <summary>
        /// The placed object type
        /// </summary>
        protected internal PlacedObjectTemplate placedObjectType;

        /// <summary>
        /// The grid positions
        /// </summary>
        protected internal List<Vector3> gridPositions;
        
        /// <summary>
        /// The direction
        /// </summary>
        protected Direction direction;

        /// <summary>
        /// The construction job
        /// </summary>
        private Job _constructionJob;

        /// <summary>
        /// The remaining work
        /// </summary>
        protected int remainingWork;

        /// <summary>
        /// The constructing
        /// </summary>
        private bool _constructing;

        /// <summary>
        /// The construction speed
        /// </summary>
        private float _constructionSpeed;

        /// <summary>
        /// The construction timer
        /// </summary>
        private float _constructionTimer;

        /// <summary>
        /// The map
        /// </summary>
        protected LocalMap map;

        /// <summary>
        /// The pawn
        /// </summary>
        private Pawn _pawn;
        
        /// <summary>
        /// Gets or sets the value of the grid objects
        /// </summary>
        public List<GridObject> GridObjects { get; internal set; }
        
        /// <summary>
        /// Gets the value of the sprite renderer
        /// </summary>
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        /// <summary>
        /// Gets the value of the needs to be made
        /// </summary>
        public bool NeedsToBeMade => remainingWork > 0;

        /// <summary>
        /// Updates this instance
        /// </summary>
        private void Update()
        {
            if (!_constructing)
            {
                return;
            }
            
            if(remainingWork <= 0)
            {
                _constructing = false;
                
                GridObjects.First().FinishConstruction();
            }

            _constructionTimer += UnityEngine.Time.deltaTime;

            if (_constructionTimer >= _constructionSpeed)
            {
                _constructionTimer -= _constructionSpeed;
                
                remainingWork--;

                //todo progress bar of some kind
            }
        }

        /// <summary>
        /// Gets the grid positions using the specified origin
        /// </summary>
        /// <param name="origin">The origin</param>
        /// <param name="dir">The dir</param>
        /// <returns>The grid position list</returns>
        protected List<Vector3> GetGridPositions(Vector2Int origin, Direction dir)
        {
            var gridPositionList = new List<Vector3>();
            
            switch (dir.Type)
            {
                default:
                case Direction.Types.DOWN:
                    for (var x = 0; x < placedObjectType.width; x++)
                    {
                        for (var y = 0; y < placedObjectType.height; y++)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) + new Vector3(x, y));
                        }
                    }

                    break;
                
                case Direction.Types.UP:
                    for (var x = placedObjectType.width - 1; x >= 0; x--)
                    {
                        for (var y = placedObjectType.height - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) - new Vector3(x, y));
                        }
                    }

                    break;
                
                case Direction.Types.LEFT:
                    for (var x = 0; x < placedObjectType.height; x++)
                    {
                        for (var y = 0; y < placedObjectType.width; y++)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) + new Vector3(x, y));
                        }
                    }

                    break;
                
                case Direction.Types.RIGHT:
                    for (var x = placedObjectType.height - 1; x >= 0; x--)
                    {
                        for (var y = placedObjectType.width - 1; y >= 0; y--)
                        {
                            gridPositionList.Add(new Vector3(origin.x, origin.y) - new Vector3(x, y));
                        }
                    }
                    
                    break;
            }

            return gridPositionList;
        }

        /// <summary>
        /// Finishes the construction
        /// </summary>
        public virtual void FinishConstruction()
        {
            _constructing = false;
            
            remainingWork = 0;
            
            MovePawnsOutTheWay();
            
            SpriteRenderer.sprite = placedObjectType.builtTexture;
            
            SpriteRenderer.color = BuiltColor;

            var buckets = new List<LayerGridBucket>();

            foreach (var gridObject in GridObjects)
            {
                gridObject.IsWalkable = placedObjectType.walkable;
                
                gridObject.IsTransparent = placedObjectType.transparent;

                var bucket = map.GetBucketAt(gridObject.Position, MapLayer.GridObject);

                if (buckets.Contains(bucket))
                {
                    continue;
                }
                
                buckets.Add(bucket);
            }

            if (placedObjectType.walkable)
            {
                return;
            }

            UnityUtils.AddBoxColliderTo(instance.gameObject);

            foreach (var bucket in buckets)
            {
                map.UpdateAStar(bucket);
            }
        }

        /// <summary>
        /// Constructs the job
        /// </summary>
        /// <param name="job">The job</param>
        /// <param name="jobPawn">The job pawn</param>
        /// <param name="skillLevel">The skill level</param>
        public void Construct(Job job, Pawn jobPawn, int skillLevel)
        {
            _constructionJob = job;
            
            _constructionJob.onPawnUnassigned += PauseConstruction;

            _pawn = jobPawn;

            _pawn.onPawnMoved += OnPawnMoved;

            _constructionSpeed =  0.5f / (skillLevel + 1);  //todo probably could use a curve for this

            _constructionTimer = 0;
            
            _constructing = true;
        }

        /// <summary>
        /// Ons the pawn moved
        /// </summary>
        private void OnPawnMoved()
        {
            _pawn.onPawnMoved -= OnPawnMoved;
            
            _pawn.CancelCurrentJob();
        }

        /// <summary>
        /// Pauses the construction using the specified job
        /// </summary>
        /// <param name="job">The job</param>
        private void PauseConstruction(Job job)
        {
            _constructing = false;
            
            _constructionJob.onPawnUnassigned -= PauseConstruction;
        }

        /// <summary>
        /// Moves the pawns out the way
        /// </summary>
        protected void MovePawnsOutTheWay()
        {
            foreach (var gridObject in GridObjects)
            {
                foreach (var mapPawn in map.GetAllPawns())
                {
                    if (gridObject.Position != mapPawn.Position)
                    {
                        continue;
                    }

                    var pawnNeighbors = map.GetAdjacentWalkableLocations(mapPawn.Position);

                    foreach (var neighbor in pawnNeighbors)
                    {
                        var nGridObject = map.GetGridObjectAt(neighbor);

                        if (nGridObject == null)
                        {
                            mapPawn.MoveToLocal(neighbor);
                            break;
                        }

                        if (GridObjects.Contains(nGridObject))
                        {
                            continue;
                        }

                        mapPawn.MoveToLocal(neighbor);
                        break;
                    }
                }
            }
            
            //todo might need to hail mary move somewhere if they get to this point. Not an issue yet.
        }
    }
}
