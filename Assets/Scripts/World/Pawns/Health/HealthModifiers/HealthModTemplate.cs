using UnityEngine;

namespace Assets.Scripts.World.Pawns.Health.HealthModifiers
{
    [CreateAssetMenu(menuName = "Templates/HealthModTemplate")]
    public class HealthModTemplate : Template
    {
        //todo Health Mod Components

        public float initialSeverity = 0.5f;

        public float lethalSeverity = -1f;

        //todo Health Mod Stages

        public bool tendable;

        public float chanceToCauseNoPain;

        public float minSeverity;

        public float maxSeverity = float.MaxValue;

        public float painPerSeverity = 1f;

        public float bleedRate = 1f;
    }
}
