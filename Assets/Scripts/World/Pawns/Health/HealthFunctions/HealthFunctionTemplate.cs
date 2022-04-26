using System;
using Assets.Scripts.World;
using Assets.Scripts.World.Pawns;
using Sirenix.Utilities;
using UnityEngine;
using World.Pawns.Health.HealthFunctions.Workers;

namespace World.Pawns.Health.HealthFunctions
{
    [CreateAssetMenu(menuName = "Templates/HealthFunctionTemplate")]
    public class HealthFunctionTemplate : Template
    {
        public Type workerType;

        public string machineLabel;

        public string animalLabel;

        public bool showOnHumanoids = true;

        public bool showOnAnimals = true;

        public bool showOnMachines;

        public bool criticalOrganic;

        public bool criticalMachines;

        public float functionalMin;

        public bool zeroIfCannotWakeUp;

        private HealthFunctionWorker _worker;

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
        
        public string GetLabelFor(Pawn pawn)
        {
            return GetLabelFor(pawn.IsOrganic, pawn.IsHumanoid);
        }

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
