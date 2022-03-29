using System;
using System.Collections.Generic;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Assets.Scripts.World.Pawns.BodyTemplates
{
    [CreateAssetMenu(menuName = "Templates/BodyTemplate")]
    public class BodyTemplate : Template
    {
        public struct Part
        {
            [TabGroup("Tabs", "$selfTabLabel")]
            [LabelText("@self?.name")]
            public BodyPartTemplate self;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public string selfTabLabel;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public string customLabel;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public BodyPartHeight.BodyPartHeight height;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public BodyPartDepth.BodyPartDepth depth;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public List<BodyPartGroupTemplate> groups;

            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public float coverage;

            [NonSerialized, OdinSerialize]
            [TabGroup("Tabs", "Children")]
            public List<Part> children;
        }

        public BodyPartTemplate corePart;

        public List<Part> parts;

        
    }
}
