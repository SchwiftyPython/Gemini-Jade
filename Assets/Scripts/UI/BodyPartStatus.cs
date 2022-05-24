using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    public class BodyPartStatus : MonoBehaviour
    {
        [SerializeField] private HealthModUi healthModPrefab;
        [SerializeField] private Transform healthModParent;

        private BodyPart _part;

        public TextMeshProUGUI partName;

        public void Setup(BodyPart part, List<HealthMod> healthMods)
        {
            foreach (Transform child in healthModParent)
            {
                Destroy(child.gameObject);
            }
            
            _part = part;

            partName.text = _part.LabelCapitalized;

            foreach (var healthMod in healthMods)
            {
                var healthModUiUi = Instantiate(healthModPrefab, healthModParent);
                healthModUiUi.Setup(healthMod);
            }
        }
    }
}
