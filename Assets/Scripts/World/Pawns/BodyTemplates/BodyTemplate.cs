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
    /// <summary>
    /// The body template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/BodyTemplate")]
    public class BodyTemplate : Template
    {
        /// <summary>
        /// The part
        /// </summary>
        public struct Part
        {
            /// <summary>
            /// The self
            /// </summary>
            [TabGroup("Tabs", "$selfTabLabel")]
            [LabelText("@self?.name")]
            public BodyPartTemplate self;

            /// <summary>
            /// The self tab label
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public string selfTabLabel;

            /// <summary>
            /// The custom label
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public string customLabel;

            /// <summary>
            /// The height
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public BodyPartHeight.BodyPartHeight height;

            /// <summary>
            /// The depth
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public BodyPartDepth.BodyPartDepth depth;

            /// <summary>
            /// The groups
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public List<BodyPartGroupTemplate> groups;

            /// <summary>
            /// The coverage
            /// </summary>
            [FoldoutGroup("Tabs/$selfTabLabel/Attributes")]
            public float coverage;

            /// <summary>
            /// The children
            /// </summary>
            [NonSerialized, OdinSerialize]
            [TabGroup("Tabs", "Children")]
            public List<Part> children;
        }

        /// <summary>
        /// The core part
        /// </summary>
        public BodyPartTemplate corePart;

        /// <summary>
        /// The parts
        /// </summary>
        public List<Part> parts;
        
        /// <summary>
        /// Describes whether this instance has parts with tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The bool</returns>
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

        /// <summary>
        /// Gets the all parts
        /// </summary>
        /// <returns>A list of part</returns>
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

        /// <summary>
        /// Gets the child parts using the specified parent
        /// </summary>
        /// <param name="parent">The parent</param>
        /// <returns>The child parts</returns>
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
