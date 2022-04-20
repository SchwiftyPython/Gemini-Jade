using Assets.Scripts.World.Pawns.Health.HealthModifiers;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
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
