using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using World.Pawns.Health;

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
        
        public bool HasPartsWithTag(BodyPartTagTemplate tag)
        {
            var allParts = GetAllParts();
            
            foreach (var bodyPart in allParts)
            {
                if (bodyPart.self.tags.Contains(tag))
                {
                    return true;
                }
            }

            return false;
        }

        public List<Part> GetAllParts()
        {
            var allParts = new List<Part>();

            foreach (var bodyPart in parts)
            {
                if (allParts.Contains(bodyPart))
                {
                    continue;
                }
                
                allParts.Add(bodyPart);

                var children = GetChildParts(bodyPart);
                
                allParts.AddRange(children);
            }

            return allParts.Distinct().ToList();
        }

        private static IEnumerable<Part> GetChildParts(Part parent)
        {
            var childParts = new List<Part>();

            foreach (var childPart in parent.children)
            {
                if (childParts.Contains(childPart))
                {
                    continue;
                }
                
                childParts.Add(childPart);

                if (childPart.children == null || childPart.children.Any())
                {
                    continue;
                }

                var children = GetChildParts(childPart);
                
                childParts.AddRange(children);
            }

            return childParts;
        }
    }
}
