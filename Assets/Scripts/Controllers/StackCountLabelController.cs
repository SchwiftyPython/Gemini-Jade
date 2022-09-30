using System.Collections.Generic;
using UI;
using UnityEngine;
using Utilities;
using World.Things;

namespace Controllers
{
    public class StackCountLabelController : MonoBehaviour
    {
        private static readonly GameObjectPool GoPool = new();

        public List<GameObject> activeLabels = new();

        private void Awake()
        {
            if (GoPool.goQueue.Count != 0)
            {
                return;
            }

            var label = new GameObject("Stack Count Label");
            
            label.transform.SetParent(transform);

            label.AddComponent<LabelComponent>();
                
            GoPool.AddFromClone(label, transform, 100);
        }

        public void AddLabel(StackThing stackThing)
        {
            var label = GoPool.GetGameObject();
            
            label.GetComponent<LabelComponent>().SetStackThing(stackThing);
        }
    }
}
