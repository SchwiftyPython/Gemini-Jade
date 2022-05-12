using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

namespace World.Pawns.Health.HealthModifiers
{
    [CreateAssetMenu(menuName = "Templates/HealthModTemplate")]
    public class HealthModTemplate : Template
    {
        //todo Health Mod Components

        public Type healthModClass;

        public float initialSeverity = 0.001f;

        public float lethalSeverity = -1f;

        public List<HealthModStage> stages;

        public bool tendable;

        public float chanceToCauseNoPain;

        public float minSeverity;

        public float maxSeverity = float.MaxValue;

        public float painPerSeverity = 1f;

        public float bleedRate = 1f;
    }
}
