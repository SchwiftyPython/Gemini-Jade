using UnityEngine;

namespace TickerTypes
{
    [CreateAssetMenu(menuName = "Enums/TickerType")]
    public class TickerType : ScriptableObject
    {
        public int tickInterval;
    }
}
