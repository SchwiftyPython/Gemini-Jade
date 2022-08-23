using Assets.Scripts.Utilities;
using Sirenix.OdinInspector;

namespace Assets.Scripts.World
{
    /// <summary>
    /// The template class
    /// </summary>
    /// <seealso cref="SerializedScriptableObject"/>
    public class Template : SerializedScriptableObject
    {
        /// <summary>
        /// The template name
        /// </summary>
        public string templateName;
        /// <summary>
        /// The label
        /// </summary>
        public string label;
        /// <summary>
        /// The description
        /// </summary>
        public string description;

        /// <summary>
        /// Gets the value of the label cap
        /// </summary>
        public virtual string LabelCap => string.IsNullOrWhiteSpace(label) ? null : label.CapitalizeFirst();
    }
}
