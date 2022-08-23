using System.Collections.Generic;
using UnityEngine;

namespace World.Pawns.Health.HealthFunctions
{
    /// <summary>
    /// The health function repo class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class HealthFunctionRepo : MonoBehaviour
    {
        /// <summary>
        /// The consciousness
        /// </summary>
        public HealthFunctionTemplate consciousness;

        /// <summary>
        /// The sight
        /// </summary>
        public HealthFunctionTemplate sight;

        /// <summary>
        /// The hearing
        /// </summary>
        public HealthFunctionTemplate hearing;

        /// <summary>
        /// The moving
        /// </summary>
        public HealthFunctionTemplate moving;

        /// <summary>
        /// The manipulation
        /// </summary>
        public HealthFunctionTemplate manipulation;

        /// <summary>
        /// The talking
        /// </summary>
        public HealthFunctionTemplate talking;

        /// <summary>
        /// The eating
        /// </summary>
        public HealthFunctionTemplate eating;

        /// <summary>
        /// The breathing
        /// </summary>
        public HealthFunctionTemplate breathing;

        /// <summary>
        /// The blood filtration
        /// </summary>
        public HealthFunctionTemplate bloodFiltration;

        /// <summary>
        /// The blood pumping
        /// </summary>
        public HealthFunctionTemplate bloodPumping;

        /// <summary>
        /// The metabolism
        /// </summary>
        public HealthFunctionTemplate metabolism;

        /// <summary>
        /// Gets the all health functions
        /// </summary>
        /// <returns>The functions</returns>
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
