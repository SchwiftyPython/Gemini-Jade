using UnityEngine;

namespace Time.TickerTypes
{
    [CreateAssetMenu(menuName = "Enums/TickerType")]
    public class TickerType : ScriptableObject
    {
        public int tickInterval;
    }
}
