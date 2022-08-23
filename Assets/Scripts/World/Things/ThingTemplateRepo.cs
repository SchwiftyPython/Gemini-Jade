using System.Collections.Generic;
using UnityEngine;

namespace World.Things
{
    /// <summary>
    /// The thing template repo class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class ThingTemplateRepo : MonoBehaviour
    {
        //todo dictionary for all lists of templates

        /// <summary>
        /// The melee weapons
        /// </summary>
        [SerializeField] private List<ThingTemplate> meleeWeapons;

        /// <summary>
        /// The knife template test
        /// </summary>
        public ThingTemplate knifeTemplateTest;
    }
}
