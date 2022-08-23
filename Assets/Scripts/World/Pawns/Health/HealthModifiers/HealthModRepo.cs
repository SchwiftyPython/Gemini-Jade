using UnityEngine;

namespace World.Pawns.Health.HealthModifiers
{
    /// <summary>
    /// The health mod repo class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthModRepo : MonoBehaviour
    {
        /// <summary>
        /// The missing body part
        /// </summary>
        public HealthModTemplate missingBodyPart;
        
        /// <summary>
        /// The blood loss
        /// </summary>
        public HealthModTemplate bloodLoss;
        
        /// <summary>
        /// The cut
        /// </summary>
        public HealthModTemplate cut;
    }
}
