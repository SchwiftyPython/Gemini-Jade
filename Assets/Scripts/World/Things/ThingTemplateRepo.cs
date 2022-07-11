using System.Collections.Generic;
using UnityEngine;

namespace World.Things
{
    public class ThingTemplateRepo : MonoBehaviour
    {
        //todo dictionary for all lists of templates

        [SerializeField] private List<ThingTemplate> meleeWeapons;

        public ThingTemplate knifeTemplateTest;
    }
}
