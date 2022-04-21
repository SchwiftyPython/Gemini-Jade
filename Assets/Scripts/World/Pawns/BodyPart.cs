using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Assets.Scripts.World.Pawns.BodyTemplates;
using Assets.Scripts.World.Pawns.Health.HealthModifiers;
using World.Pawns.Health.HealthModifiers;

namespace Assets.Scripts.World.Pawns
{
    public class BodyPart
    {
        private BodyTemplate _body;

        private List<HealthMod> _healthMods;

        public BodyPartTemplate template;

        private string _customLabel;

        private List<BodyPart> _children;

        private BodyPartHeight.BodyPartHeight _height;

        public BodyPartDepth.BodyPartDepth depth;

        private float _coverage;

        private List<BodyPartGroupTemplate> _groups;

        public BodyPart parent;

        public bool IsCorePart => parent == null;

        public string Label => string.IsNullOrWhiteSpace(_customLabel) ? template.label : _customLabel;

        public string LabelCapitalized => string.IsNullOrWhiteSpace(_customLabel)
            ? template.LabelCap
            : _customLabel.CapitalizeFirst();

        public BodyPart(BodyTemplate body, BodyTemplate.Part part, BodyPart parent = null)
        {
            _body = body;
            template = part.self;
            _customLabel = part.customLabel;
            _height = part.height;
           depth = part.depth;
           _coverage = part.coverage;
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

        public bool IsMissing()
        {
            if (_healthMods == null || _healthMods.Count < 1)
            {
                return false;
            }

            foreach (var mod in _healthMods)
            {
                if (mod is MissingBodyPart)
                {
                    return true;
                }
            }

            return false;
        }

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

        public bool HasChildParts(BodyPartTagTemplate tag)
        {
            return GetChildParts(tag).Any();
        }

        public bool HasChildParts()
        {
            return _children != null && _children.Any();
        }

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

        public bool HasHealthMods()
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

            hMod.PostAdd();
        }

        
    }
}
