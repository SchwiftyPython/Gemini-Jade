using UnityEngine;

namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    public class TendDuration : HealthModCompProps
    {
        private float _baseTendDurationHours = -1f;

        private float _tendOverlapHours = 3f;

        public bool tendAllAtOnce;

        public int disappearsAtTotalTendQuality = -1;

        public float severityPerDayTended;

        public bool showTendQuality = true;

        public bool TendIsPermanent => _baseTendDurationHours < 0f;
        
        public int TendTicksFull => Mathf.RoundToInt((_baseTendDurationHours + _tendOverlapHours) * 2500f); //magic number

        public int TendTicksBase => Mathf.RoundToInt(_baseTendDurationHours * 2500f); //magic number

        public int TendTicksOverlap => Mathf.RoundToInt(_tendOverlapHours * 2500f); //magic number

        public TendDuration()
        {
            compClass = typeof(HealthModifierComponents.TendDuration);
        }
    }
}
