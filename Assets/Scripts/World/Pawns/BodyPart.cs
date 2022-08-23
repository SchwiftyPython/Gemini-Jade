using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Assets.Scripts.World.Pawns.BodyTemplates;

namespace Assets.Scripts.World.Pawns
{
    /// <summary>
    /// The body part class
    /// </summary>
    public class BodyPart
    {
        /// <summary>
        /// The body
        /// </summary>
        private BodyTemplate _body;

        /// <summary>
        /// The template
        /// </summary>
        public BodyPartTemplate template;

        /// <summary>
        /// The custom label
        /// </summary>
        private string _customLabel;

        /// <summary>
        /// The children
        /// </summary>
        private List<BodyPart> _children;

        /// <summary>
        /// The height
        /// </summary>
        public BodyPartHeight.BodyPartHeight height;

        /// <summary>
        /// The depth
        /// </summary>
        public BodyPartDepth.BodyPartDepth depth;

        /// <summary>
        /// The coverage
        /// </summary>
        public float coverage;

        /// <summary>
        /// The groups
        /// </summary>
        private List<BodyPartGroupTemplate> _groups;

        /// <summary>
        /// The parent
        /// </summary>
        public BodyPart parent;

        /// <summary>
        /// Gets the value of the is core part
        /// </summary>
        public bool IsCorePart => parent == null;

        /// <summary>
        /// Gets the value of the label
        /// </summary>
        public string Label => string.IsNullOrWhiteSpace(_customLabel) ? template.label : _customLabel;

        /// <summary>
        /// Gets the value of the label capitalized
        /// </summary>
        public string LabelCapitalized => string.IsNullOrWhiteSpace(_customLabel)
            ? template.LabelCap
            : _customLabel.CapitalizeFirst();

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyPart"/> class
        /// </summary>
        /// <param name="body">The body</param>
        /// <param name="part">The part</param>
        /// <param name="parent">The parent</param>
        public BodyPart(BodyTemplate body, BodyTemplate.Part part, BodyPart parent = null)
        {
            _body = body;
            template = part.self;
            _customLabel = part.customLabel;
            height = part.height;
           depth = part.depth;
           coverage = part.coverage;
           _groups = new List<BodyPartGroupTemplate>(part.groups);
           this.parent = parent;

           _children = new List<BodyPart>();

           if (part.children == null || part.children.Count < 1)
           {
               return;
           }

           foreach (var cPart in part.children.ToArray())
           {
               var child = new BodyPart(body, cPart, this);

               _children.Add(child);
           }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyPart"/> class
        /// </summary>
        public BodyPart()
        {
            _customLabel = "Whole Body";
        }

        /// <summary>
        /// Describes whether this instance is in group
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>The bool</returns>
        public bool IsInGroup(BodyPartGroupTemplate group)
        {
            foreach (var g in _groups)
            {
                if (g == group)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the all children
        /// </summary>
        /// <returns>The all children</returns>
        public List<BodyPart> GetAllChildren()
        {
            var allChildren = new List<BodyPart>();

            allChildren.AddRange(_children);

            foreach (var child in _children)
            {
                allChildren.AddRange(child.GetAllChildren());
            }

            return allChildren;
        }

        /// <summary>
        /// Gets the child parts using the specified tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>An enumerable of body part</returns>
        public IEnumerable<BodyPart> GetChildParts(BodyPartTagTemplate tag)
        {
            if (template.tags.Contains(tag))
            {
                yield return this;
            }
            var i = 0;
            while (i < _children.Count)
            {
                foreach (var childPart in _children[i].GetChildParts(tag))
                {
                    yield return childPart;
                }
                var num = i + 1;
                i = num;
            }
        }

        /// <summary>
        /// Gets the direct child parts
        /// </summary>
        /// <returns>An enumerable of body part</returns>
        public IEnumerable<BodyPart> GetDirectChildParts()
        {
            var i = 0;
            while (i < _children.Count)
            {
                yield return _children[i];
                var num = i + 1;
                i = num;
            }
        }

        /// <summary>
        /// Describes whether this instance has child parts
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>The bool</returns>
        public bool HasChildParts(BodyPartTagTemplate tag)
        {
            return GetChildParts(tag).Any();
        }

        /// <summary>
        /// Describes whether this instance has child parts
        /// </summary>
        /// <returns>The bool</returns>
        public bool HasChildParts()
        {
            return _children != null && _children.Any();
        }

        /// <summary>
        /// Gets the connected parts using the specified tag
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>An enumerable of body part</returns>
        public IEnumerable<BodyPart> GetConnectedParts(BodyPartTagTemplate tag)
        {
            var bodyPart = this;
            while (bodyPart.parent != null && bodyPart.parent.template.tags.Contains(tag))
            {
                bodyPart = bodyPart.parent;
            }
            foreach (var childPart in bodyPart.GetChildParts(tag))
            {
                yield return childPart;
            }
        }

        /*public bool HasHealthMods()
        {
            return _healthMods != null && _healthMods.Any();
        }

        public List<HealthMod> GetHealthMods()
        {
            return _healthMods;
        }

        public void AddHealthMod(HealthMod hMod)
        {
            if (_healthMods == null)
            {
                _healthMods = new List<HealthMod>();
            }

            _healthMods.Add(hMod);
        }

        public void RemoveHealthMod(HealthMod hMod)
        {
            if (_healthMods == null)
            {
                return;
            }

            _healthMods.Remove(hMod);
        }*/
    }
}
