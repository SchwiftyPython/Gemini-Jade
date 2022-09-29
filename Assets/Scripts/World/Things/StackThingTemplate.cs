using UnityEngine;

namespace World.Things
{
    public class StackThingTemplate : ThingTemplate
    {
        /// <summary>
        /// The stack limit
        /// </summary>
        [SerializeField] private int stackLimit = -1;
    
        /// <summary>
        /// Gets the value of the stack limit
        /// </summary>
        public int StackLimit
        {
            get
            {
                if (stackLimit != -1)
                {
                    return stackLimit;
                }
                
                return parent != null ? ((StackThingTemplate)parent).StackLimit : stackLimit;
            }
        }
    }
}
