using UnityEngine;

namespace World.Pawns.Health.HealthModifierComponents.HealthModCompProperties
{
    /// <summary>
    /// The tend duration class
    /// </summary>
    /// <seealso cref="HealthModCompProps"/>
    public class TendDuration : HealthModCompProps
    {
        /// <summary>
        /// The base tend duration hours
        /// </summary>
        private float _baseTendDurationHours = -1f;

        /// <summary>
        /// The tend overlap hours
        /// </summary>
        private float _tendOverlapHours = 3f;

        /// <summary>
        /// The tend all at once
        /// </summary>
        public bool tendAllAtOnce;

        /// <summary>
        /// The disappears at total tend quality
        /// </summary>
        public int disappearsAtTotalTendQuality = -1;

        /// <summary>
        /// The severity per day tended
        /// </summary>
        public float severityPerDayTended;

        /// <summary>
        /// The show tend quality
        /// </summary>
        public bool showTendQuality = true;

        /// <summary>
        /// Gets the value of the tend is permanent
        /// </summary>
        public bool TendIsPermanent => _baseTendDurationHours < 0f;
        
        /// <summary>
        /// Gets the value of the tend ticks full
        /// </summary>
        public int TendTicksFull => Mathf.RoundToInt((_baseTendDurationHours + _tendOverlapHours) * 2500f); //magic number

        /// <summary>
        /// Gets the value of the tend ticks base
        /// </summary>
        public int TendTicksBase => Mathf.RoundToInt(_baseTendDurationHours * 2500f); //magic number

        /// <summary>
        /// Gets the value of the tend ticks overlap
        /// </summary>
        public int TendTicksOverlap => Mathf.RoundToInt(_tendOverlapHours * 2500f); //magic number

        /// <summary>
        /// Initializes a new instance of the <see cref="TendDuration"/> class
        /// </summary>
        public TendDuration()
        {
            compClass = typeof(HealthModifierComponents.TendDuration);
        }
    }
}
