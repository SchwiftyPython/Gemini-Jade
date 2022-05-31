using Assets.Scripts.Utilities;
using Sirenix.OdinInspector;

namespace Assets.Scripts.World
{
    public class Template : SerializedScriptableObject
    {
        public string templateName;
        public string label;
        public string description;

        public virtual string LabelCap => string.IsNullOrWhiteSpace(label) ? null : label.CapitalizeFirst();
    }
}
