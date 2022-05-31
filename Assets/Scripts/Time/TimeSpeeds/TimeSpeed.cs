using UnityEngine;

namespace Time.TimeSpeeds
{
    [CreateAssetMenu(menuName = "Enums/TimeSpeed")]
    public class TimeSpeed : ScriptableObject
    {
        public float tickRateMultiplier;
    }
}
