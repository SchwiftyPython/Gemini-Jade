using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.BodyTemplates
{
    [CreateAssetMenu(menuName = "Templates/BodyTemplate")]
    public class BodyTemplate : Template
    {
        public BodyPartTemplate corePart;

        public List<Dictionary<BodyPartTemplate, List<BodyPartTemplate>>> parts =
            new List<Dictionary<BodyPartTemplate, List<BodyPartTemplate>>>();
    }
}
