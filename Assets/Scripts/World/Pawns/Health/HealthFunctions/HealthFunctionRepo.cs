using System.Collections.Generic;
using UnityEngine;

namespace World.Pawns.Health.HealthFunctions
{
    public class HealthFunctionRepo : MonoBehaviour
    {
        public HealthFunctionTemplate consciousness;

        public HealthFunctionTemplate sight;

        public HealthFunctionTemplate hearing;

        public HealthFunctionTemplate moving;

        public HealthFunctionTemplate manipulation;

        public HealthFunctionTemplate talking;

        public HealthFunctionTemplate eating;

        public HealthFunctionTemplate breathing;

        public HealthFunctionTemplate bloodFiltration;

        public HealthFunctionTemplate bloodPumping;

        public HealthFunctionTemplate metabolism;

        public List<HealthFunctionTemplate> GetAllHealthFunctions()
        {
            var functions = new List<HealthFunctionTemplate>
            {
                consciousness,
                sight,
                hearing,
                moving,
                manipulation,
                talking,
                eating,
                breathing,
                bloodFiltration,
                bloodPumping,
                metabolism
            };

            return functions;
        }
    }
}
