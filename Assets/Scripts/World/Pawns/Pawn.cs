using System.Collections.Generic;
using Assets.Scripts.World.Pawns.Species;

namespace Assets.Scripts.World.Pawns
{
    public class Pawn
    {
        public SpeciesTemplate species;

        private string _name;

        private Health.Health _health;

        public Pawn(SpeciesTemplate speciesTemplate)
        {
            species = speciesTemplate;

            _name = "Testy McTestes";

            _health = new Health.Health(this);
        }

        public List<BodyPart> GetBody()
        {
            return _health.GetBody();
        }
    }
}
