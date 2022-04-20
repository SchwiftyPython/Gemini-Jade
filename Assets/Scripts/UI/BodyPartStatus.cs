using Assets.Scripts.World.Pawns;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class BodyPartStatus : MonoBehaviour
    {
        [SerializeField] private HealthModUi _healthModPrefab;
        [SerializeField] private Transform _healthModParent;

        private BodyPart _part;

        public TextMeshProUGUI partName;

        public void Setup(BodyPart part)
        {
            _part = part;

            partName.text = _part.LabelCapitalized;

            foreach (Transform child in _healthModParent)
            {
                Destroy(child.gameObject);
            }

            if (!part.HasHealthMods())
            {
                return;
            }

            foreach (var healthMod in part.GetHealthMods())
            {
                //todo can probably just show highest level parent that's missing instead of all parts
                //todo like shoulder is missing so of course arm, fingers, etc are missing too

                var healthModUiUi = Instantiate(_healthModPrefab, _healthModParent);
                healthModUiUi.Setup(healthMod);
            }
        }
    }
}
