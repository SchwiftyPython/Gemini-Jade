using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    public class HealthModUi : MonoBehaviour
    {
        private HealthMod _healthMod;

        public TextMeshProUGUI modName;

        public void Setup(HealthMod hMod)
        {
            _healthMod = hMod;

            modName.text = _healthMod.LabelBase;
        }
    }
}
