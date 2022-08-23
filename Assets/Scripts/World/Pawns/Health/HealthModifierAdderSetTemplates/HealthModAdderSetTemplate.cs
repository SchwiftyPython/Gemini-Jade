using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Health.HealthModifierAdders;

namespace World.Pawns.Health.HealthModifierAdderSetTemplates
{
    /// <summary>
    /// The health mod adder set template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/HealthModAdderSetTemplate")]
    public class HealthModAdderSetTemplate : Template
    {
        /// <summary>
        /// The health mod adders
        /// </summary>
        public List<HealthModAdder> healthModAdders;
    }
}
