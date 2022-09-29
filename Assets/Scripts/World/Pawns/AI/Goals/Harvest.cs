using System.Collections.Generic;
using System.Linq;
using GoRogue;
using UnityEngine;
using Utilities;
using World.Things.Plants;

namespace World.Pawns.AI.Goals
{
    public class Harvest : JobGoal
    {
        /// <summary>
        /// The work spot
        /// </summary>
        private Coord _workSpot;

        private List<Coord> _possibleWorkSpots;

        private bool _foundWorkSpot;

        private Plant _plant;

        private bool _finished;
        
        public Harvest() {}

        public override void TakeAction()
        {
            _inProgress = true;

            var plant = ((LocalMap) Pawn.CurrentMap).GetPlantAt(Job.Location);

            if (_workSpot == Coord.NONE || Vector3.Distance(Pawn.TransformPosition, _workSpot.ToVector3()) >= 1f)
            {
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
            
            plant.Harvest(Job, Pawn);

            plant.onJobFinished += HarvestFinished;
        }

        private void HarvestFinished()
        {
            _finished = true;
        }

        /// <summary>
        /// Failed goal
        /// </summary>
        public override void Failed()
        {
            Job.UnAssignPawn();
            
            Pop();
        }

        public override bool Finished()
        {
            return _finished;
        }
    }
}
