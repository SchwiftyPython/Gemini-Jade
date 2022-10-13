using UI;
using UnityEngine;
using Utilities;
using World.Things;

namespace Controllers
{
    public class StackCountLabelController : MonoBehaviour
    {
        private static readonly GameObjectPool GoPool = new();

        private void Awake()
        {
            if (GoPool.goQueue.Count != 0)
            {
                return;
            }

            var labelParent = new GameObject("Stack Count Label");

            var boxCollider = labelParent.AddComponent<BoxCollider>();

            boxCollider.isTrigger = true;

            boxCollider.center = new Vector3(0, 0.25f, 0);

            boxCollider.size = new Vector3(2, 2, 2);

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
        }
    }
}
