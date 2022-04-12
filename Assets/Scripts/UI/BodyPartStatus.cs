using Assets.Scripts.World.Pawns;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class BodyPartStatus : MonoBehaviour
    {
        [SerializeField] private HealthMod _healthModPrefab;
        [SerializeField] private Transform _healthModParent;

        private BodyPart _part;

        public TextMeshProUGUI partName;

        public void Setup(BodyPart part)
        {
            _part = part;

            partName.text = _part.LabelCapitalized;

            //todo health mods
        }
    }
}
