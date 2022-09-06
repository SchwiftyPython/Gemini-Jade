using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoRogue;
using UnityEngine;
using Utilities;

namespace World.Pawns.AI.Goals
{
    /// <summary>
    /// The build class
    /// </summary>
    /// <seealso cref="JobGoal"/>
    public class Build : JobGoal
    {
        /// <summary>
        /// The blueprint
        /// </summary>
        private GridObject _blueprint;
        
        /// <summary>
        /// The work spot
        /// </summary>
        private Coord _workSpot;

        private List<Coord> _possibleWorkSpots;

        private bool _foundWorkSpot;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Build"/> class
        /// </summary>
        public Build()
        {
        }

        /// <summary>
        /// Takes the action
        /// </summary>
        public override void TakeAction()
        {
            _inProgress = true;
            
            _blueprint = ((LocalMap) Pawn.CurrentMap).GetBlueprintAt(Job.Location);
            
            //todo if there are resources available, haul them to the location -- add haul child goal
        
            //todo if no resources available, fail to parent -- this should suspend that job until more resources are available
            // basically job subscribes to a stockpile event for that particular resource
        
            //todo if all resources are added, construct at some rate based on construction skill until work left is zero

            if (Vector3.Distance(Pawn.TransformPosition, _workSpot.ToVector3()) >= 1f)
            {
                //todo this works for one tile jobs, but for bigger blueprints we need to find adjacent tiles around whole object
                // maybe loop through all grid objects and return walkable adjacent coords that are not part of the same placed object
                //changing job location to list to make this easier

                if (!_foundWorkSpot || !Pawn.Movement.CanPathTo(_workSpot))
                {
                    _possibleWorkSpots = (List<Coord>) ((LocalMap) Pawn.CurrentMap).GetAdjacentWalkableLocations(Job.Location);

                    _workSpot = Coord.NONE;

                    _foundWorkSpot = false;

                    foreach (var position in _possibleWorkSpots.ShuffleIterator().ToArray()) 
                    {
                        if (Pawn.Movement.CanPathTo(position.ToVector3()))
                        {
                            _workSpot = position;
                            _foundWorkSpot = true;
                            break;
                        }

                        _possibleWorkSpots.Remove(position);
                    }
                }

                if (!_foundWorkSpot) 
                {
                    //todo suspend job until a walkable neighbor is available
                    
                    Debug.Log($"{Pawn.id} cannot path to {Job.Location}");
                    
                    Failed();
                    
                    return;
                }
                
                var localMove = new LocalMove(Pawn.Movement, _workSpot);
                
                PushChildGoal(localMove);

                _inProgress = false;

                return;
            }
            
            Pawn.FaceToward(Job.Location);

            _blueprint.PlacedObject.Construct(Job, Pawn,16); //todo testing 
        }

        /// <summary>
        /// Faileds this instance
        /// </summary>
        public override void Failed()
        {
            Job.UnAssignPawn();
            
            Pop();
        }

        /// <summary>
        /// Describes whether this instance finished
        /// </summary>
        /// <returns>The bool</returns>
        public override bool Finished()
        {
            if (_blueprint == null)
            {
                return false;
            }
            
            return !_blueprint.PlacedObject.NeedsToBeMade;
        }
    }
}
