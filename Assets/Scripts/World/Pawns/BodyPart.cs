using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;
using Assets.Scripts.World.Pawns.BodyPartGroupTemplates;
using Assets.Scripts.World.Pawns.BodyPartTags;
using Assets.Scripts.World.Pawns.BodyPartTemplates;
using Assets.Scripts.World.Pawns.BodyTemplates;

namespace Assets.Scripts.World.Pawns
{
    public class BodyPart
    {
        private BodyTemplate _body;

        private BodyPartTemplate _template;

        private string _customLabel;

        private List<BodyPart> _children;

        private BodyPartHeight.BodyPartHeight _height;

        private BodyPartDepth.BodyPartDepth _depth;

        private float _coverage;

        private List<BodyPartGroupTemplate> _groups;

        private BodyPart _parent;

        public bool IsCorePart => _parent == null;

        public string Label => string.IsNullOrWhiteSpace(_customLabel) ? _template.label : _customLabel;

        public string LabelCapitalized => string.IsNullOrWhiteSpace(_customLabel)
            ? _template.LabelCap
            : _customLabel.CapitalizeFirst();

        public BodyPart(BodyTemplate body, BodyTemplate.Part part, BodyPart parent = null)
        {
            _body = body;
            _template = part.self;
            _height = part.height;
           _depth = part.depth;
           _coverage = part.coverage;
           _groups = new List<BodyPartGroupTemplate>(part.groups);
           _parent = parent;

           _children = new List<BodyPart>();

           foreach (var cPart in part.children)
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

        public List<BodyPart> GetAllChildren()
        {
            var allChildren = new List<BodyPart>();

            foreach (var child in _children)
            {
                allChildren.AddRange(child.GetAllChildren());
            }

            return allChildren;
        }

        public IEnumerable<BodyPart> GetChildParts(BodyPartTagTemplate tag)
        {
            if (_template.tags.Contains(tag))
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

        public IEnumerable<BodyPart> GetConnectedParts(BodyPartTagTemplate tag)
        {
            var bodyPart = this;
            while (bodyPart._parent != null && bodyPart._parent._template.tags.Contains(tag))
            {
                bodyPart = bodyPart._parent;
            }
            foreach (var childPart in bodyPart.GetChildParts(tag))
            {
                yield return childPart;
            }
        }
    }
}
