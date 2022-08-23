using System;
using Assets.Scripts.World;
using Assets.Scripts.World.Pawns;
using Sirenix.Utilities;
using UnityEngine;
using World.Pawns.Health.HealthFunctions.Workers;

namespace World.Pawns.Health.HealthFunctions
{
    /// <summary>
    /// The health function template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/HealthFunctionTemplate")]
    public class HealthFunctionTemplate : Template
    {
        /// <summary>
        /// The worker type
        /// </summary>
        public Type workerType;

        /// <summary>
        /// The machine label
        /// </summary>
        public string machineLabel;

        /// <summary>
        /// The animal label
        /// </summary>
        public string animalLabel;

        /// <summary>
        /// The show on humanoids
        /// </summary>
        public bool showOnHumanoids = true;

        /// <summary>
        /// The show on animals
        /// </summary>
        public bool showOnAnimals = true;

        /// <summary>
        /// The show on machines
        /// </summary>
        public bool showOnMachines;

        /// <summary>
        /// The critical organic
        /// </summary>
        public bool criticalOrganic;

        /// <summary>
        /// The critical machines
        /// </summary>
        public bool criticalMachines;

        /// <summary>
        /// The functional min
        /// </summary>
        public float functionalMin;

        /// <summary>
        /// The zero if cannot wake up
        /// </summary>
        public bool zeroIfCannotWakeUp;

        /// <summary>
        /// The worker
        /// </summary>
        private HealthFunctionWorker _worker;

        /// <summary>
        /// Gets the value of the worker
        /// </summary>
        public HealthFunctionWorker Worker
        {
            get
            {
                if (_worker == null)
                {
                    _worker = (HealthFunctionWorker) Activator.CreateInstance(workerType);
                }

                return _worker;
            }
        }
        
        /// <summary>
        /// Gets the label for using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        /// <returns>The string</returns>
        public string GetLabelFor(Pawn pawn)
        {
            return GetLabelFor(pawn.IsOrganic, pawn.IsHumanoid);
        }

        /// <summary>
        /// Gets the label for using the specified is organic
        /// </summary>
        /// <param name="isOrganic">The is organic</param>
        /// <param name="isHumanoid">The is humanoid</param>
        /// <returns>The string</returns>
        public string GetLabelFor(bool isOrganic, bool isHumanoid)
        {
            if (isHumanoid)
            {
                return label;
            }

            if (isOrganic)
            {
                return !animalLabel.IsNullOrWhitespace() ? animalLabel : label;
            }

            return !machineLabel.IsNullOrWhitespace() ? machineLabel : label;
        }
        
        /// <summary>
        /// Describes whether this instance can show on pawn
        /// </summary>
        /// <param name="p">The </param>
        /// <returns>The bool</returns>
        public bool CanShowOnPawn(Pawn p)
        {
            if (p.IsHumanoid)
            {
                return showOnHumanoids;
            }
            return p.IsAnimal ? showOnAnimals : showOnMachines;
        }
    }
}
