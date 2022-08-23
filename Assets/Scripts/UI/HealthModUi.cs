using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    /// <summary>
    /// The health mod ui class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthModUi : MonoBehaviour
    {
        /// <summary>
        /// The health mod
        /// </summary>
        private HealthMod _healthMod;

        /// <summary>
        /// The mod name
        /// </summary>
        public TextMeshProUGUI modName;

        /// <summary>
        /// Setup the h mod
        /// </summary>
        /// <param name="hMod">The mod</param>
        public void Setup(HealthMod hMod)
        {
            _healthMod = hMod;

            modName.text = _healthMod.LabelBase;
        }
    }
}
