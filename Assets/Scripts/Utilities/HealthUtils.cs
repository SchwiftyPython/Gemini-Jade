using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.BodyPartDepth;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class HealthUtils : MonoBehaviour
    {
        public BodyPartDepth inside;
        public BodyPartDepth outside;
        public BodyPartDepth undefined;

        public string GetGeneralDestroyedPartLabel(BodyPart part, bool solid)
        {
            if (part.parent == null)
            {
                return "SeriouslyImpaired";
            }

            if (part.depth != inside) //todo && !fresh
            {
                return "MissingBodyPart";
            }

            return solid ? "ShatteredBodyPart" : "DestroyedBodyPart";
        }
	}
}
