using UnityEngine;
using World.Pawns;
using World.Pawns.Jobs;

namespace World
{
    /// <summary>
    /// Create an instance of this whenever needing a job to be worked on.
    /// Basic idea is to be able to take in a work amount and a work speed and keep track of progress until done.
    /// </summary>
    public class JobProgressTracker : MonoBehaviour
    {
        private Job _job;

        private Pawn _worker;
        
        private int _remainingWork;

        private bool _workingOn;

        private float _workSpeed;

        private float _workTimer;

        public bool NeedsToBeWorkedOn => _remainingWork > 0;

        private void Update()
        {
            if (!_workingOn)
            {
                return;
            }

            if (_remainingWork <= 0)
            {
                _workingOn = false;
                
                //todo notify work done
                
                Destroy(this);
                
                return;
            }

            _workTimer += UnityEngine.Time.deltaTime;

            if (_workTimer >= _workSpeed)
            {
                _workTimer -= _workSpeed;

                _remainingWork--;
                
                //todo progress bar
            }
        }

        public void WorkOn(Job job, Pawn worker, int skillLevel)
        {
            _job = job;

            _job.onPawnUnassigned += PauseWork;

            _worker = worker;

            _worker.onPawnMoved += OnPawnMoved;

            _workSpeed = 0.5f / (skillLevel + 1);

            _workTimer = 0;

            _workingOn = true;
        }
        
        /// <summary>
        /// Ons the pawn moved
        /// </summary>
        private void OnPawnMoved()
        {
            _worker.onPawnMoved -= OnPawnMoved;
            
            _worker.CancelCurrentJob();
        }
        
        /// <summary>
        /// Pauses work
        /// </summary>
        /// <param name="job">The job</param>
        private void PauseWork(Job job)
        {
            _workingOn = false;
            
            _job.onPawnUnassigned -= PauseWork;
            
            //todo might be a good idea to save progress in a repo somewhere if the number of game objects gets too high
            //can just retrieve the info again when job is being worked on and send to a new game object
        }
    }
}
