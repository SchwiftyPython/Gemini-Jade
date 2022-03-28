using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.BodyTemplates
{
    [CreateAssetMenu(menuName = "Templates/BodyTemplate")]
    public class BodyTemplate : Template
    {
        private string _selfTabLabel;

        public struct Part
        {
            [TabGroup("Tabs", "$selfTabLabel")]
            [LabelText("@self?.name")]
            public BodyPartTemplate self;

            public string selfTabLabel;

            //todo need scriptable objects for height and depth

            [NonSerialized, OdinSerialize]
            [TabGroup("Tabs", "Children")]
            public List<Part> children;
        }

        public BodyPartTemplate corePart;

        public List<Part> parts;

        
    }
}
