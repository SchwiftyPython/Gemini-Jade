using System.Collections.Generic;
using Assets.Scripts.World.Pawns;
using Assets.Scripts.World.Pawns.Species;
using World.Pawns.Health;

namespace World.Pawns
{
    public class Pawn
    {
        public SpeciesTemplate species;

        private string _name;

        public Health.Health health;

        public bool Dead => health.Dead;

        public bool CanWakeUp => health.CanWakeUp;

        public bool IsOrganic => species.fleshType != FleshType.Machine;

        public bool IsHumanoid => species.intellect >= Intellect.Humanoid;

        public bool ToolUser => species.intellect >= Intellect.ToolUser;

        public bool IsAnimal => !ToolUser && IsOrganic;

        public Pawn(SpeciesTemplate speciesTemplate)
        {
            species = speciesTemplate;

            _name = "Testy McTestes";

            health = new Health.Health(this);
        }

        public List<BodyPart> GetBody()
        {
            return health.GetBody();
        }
    }
}
