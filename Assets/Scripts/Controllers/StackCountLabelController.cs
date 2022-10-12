using System;
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

            var labelParent = new GameObject("Stack Count Label");

            var collider = labelParent.AddComponent<BoxCollider>();

            collider.isTrigger = true;

            collider.center = new Vector3(0, 0.25f, 0);

            collider.size = new Vector3(2, 2, 2);

            labelParent.AddComponent<LabelComponent>();
            
            labelParent.transform.SetParent(transform);

            var label = new GameObject("Label");

            label.transform.SetParent(labelParent.transform);
            
            label.transform.localPosition = Vector3.zero;

            GoPool.AddFromClone(labelParent, transform, 100);
        }

        public void AddLabel(StackThing stackThing)
        {
            var label = GoPool.GetGameObject();
            
            label.GetComponent<LabelComponent>().SetStackThing(stackThing);

            //Also might want to show labels in a given radius so need to grab adjacent labels too if they exist
        }
    }
}
