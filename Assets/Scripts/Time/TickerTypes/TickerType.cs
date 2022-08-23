using UnityEngine;

namespace Time.TickerTypes
{
    /// <summary>
    /// The ticker type class
    /// </summary>
    /// <seealso cref="ScriptableObject"/>
    [CreateAssetMenu(menuName = "Enums/TickerType")]
    public class TickerType : ScriptableObject
    {
        /// <summary>
        /// The tick interval
        /// </summary>
        public int tickInterval;
    }
}
