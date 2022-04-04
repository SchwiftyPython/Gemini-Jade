using UnityEngine;

namespace Assets.Scripts.TickerTypes
{
    [CreateAssetMenu(menuName = "Enums/TickerType")]
    public class TickerType : ScriptableObject
    {
        public int tickInterval;
    }
}
