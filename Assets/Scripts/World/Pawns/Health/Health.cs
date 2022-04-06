using UnityEngine;

namespace Assets.Scripts.World.Pawns.Health
{
    public class Health
    {
        private Pawn _pawn;

        private HealthState _healthState = HealthState.Mobile;

        //todo health diffs

        //todo pawn capacities

        //todo health summary calculator

        public HealthState State => _healthState;

        public bool Downed => _healthState == HealthState.Down;

        public bool Dead => _healthState == HealthState.Dead;

        public float maxLethalDamage = 150f;

        public float painShockThreshold = 0.8f; //todo define in pawn's stats

        public Health(Pawn pawn)
        {
            _pawn = pawn;

            //todo set all the other properties
        }
    }
}
