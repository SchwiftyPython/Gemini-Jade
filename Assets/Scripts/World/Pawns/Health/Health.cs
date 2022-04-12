using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.World.Pawns.Health
{
    public class Health
    {
        private Pawn _pawn;

        private HealthState _healthState = HealthState.Mobile;

        //todo health mods -- next iteration. Makes sense to only reference body parts in the health mods

        //todo pawn capacities

        //todo health summary calculator

        private List<BodyPart> _body;

        public HealthState State => _healthState;

        public bool Downed => _healthState == HealthState.Down;

        public bool Dead => _healthState == HealthState.Dead;

        public float maxLethalDamage = 150f;

        public float painShockThreshold = 0.8f; //todo define in pawn's stats

        public Health(Pawn pawn)
        {
            _pawn = pawn;

            BuildBody();

            //todo set all the other properties
        }

        public List<BodyPart> GetBody()
        {
            return _body;
        }

        private void BuildBody()
        {
            //todo I think we just need to build the core part and that will build the whole body

            var corePartInfo = _pawn.species.bodyTemplate.parts.SingleOrDefault(p => p.self.templateName.Equals(_pawn.species.bodyTemplate.corePart.templateName));

            var corePart = new BodyPart(_pawn.species.bodyTemplate, corePartInfo);

            _body = corePart.GetAllChildren();
        }
    }
}
