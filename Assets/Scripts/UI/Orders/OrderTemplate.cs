using Assets.Scripts.World;
using UnityEngine;
using World.Pawns.Skills;

namespace UI.Orders
{
    [CreateAssetMenu(menuName = "Create OrderTemplate", fileName = "OrderTemplate")]
    public class OrderTemplate : Template
    {
        public Skill skillNeeded;

        public Selection selectionType;

        //idea here is to define a selection and then add a job for each of those selections
    }
}
