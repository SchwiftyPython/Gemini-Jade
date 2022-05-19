using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Health.HealthModifierAdders;

namespace World.Pawns.Health.HealthModifierAdderSetTemplates
{
    [CreateAssetMenu(menuName = "Templates/HealthModAdderSetTemplate")]
    public class HealthModAdderSetTemplate : Template
    {
        public List<HealthModAdder> healthModAdders;
    }
}
