using UnityEngine;

namespace Assets.Scripts.World.Pawns.BodyPartTags
{
    /// <summary>
    /// The body part tag template class
    /// </summary>
    /// <seealso cref="Template"/>
    [CreateAssetMenu(menuName = "Templates/BodyPartTagTemplate")]
    public class BodyPartTagTemplate : Template
    {
        /// <summary>
        /// The vital
        /// </summary>
        public bool vital;
    }
}
