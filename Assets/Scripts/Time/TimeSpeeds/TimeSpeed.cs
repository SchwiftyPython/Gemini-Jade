using UnityEngine;

namespace Time.TimeSpeeds
{
    /// <summary>
    /// The time speed class
    /// </summary>
    /// <seealso cref="ScriptableObject"/>
    [CreateAssetMenu(menuName = "Enums/TimeSpeed")]
    public class TimeSpeed : ScriptableObject
    {
        /// <summary>
        /// The tick rate multiplier
        /// </summary>
        public float tickRateMultiplier;
    }
}
