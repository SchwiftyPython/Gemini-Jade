using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using TMPro;
using UnityEngine;
using World.Pawns.Health.HealthModifiers;

namespace UI
{
    /// <summary>
    /// The body part status class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class BodyPartStatus : MonoBehaviour
    {
        /// <summary>
        /// The health mod prefab
        /// </summary>
        [SerializeField] private HealthModUi healthModPrefab;
        /// <summary>
        /// The health mod parent
        /// </summary>
        [SerializeField] private Transform healthModParent;

        /// <summary>
        /// The part
        /// </summary>
        private BodyPart _part;

        /// <summary>
        /// The part name
        /// </summary>
        public TextMeshProUGUI partName;

        /// <summary>
        /// Setup the part
        /// </summary>
        /// <param name="part">The part</param>
        /// <param name="healthMods">The health mods</param>
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
