using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class GameObjectPool
    {
        public readonly Queue<GameObject> goQueue = new();

        public GameObject GetGameObject()
        {
            var gameObject = goQueue.Dequeue();
        
            gameObject.SetActive(true);

            return gameObject;
        }

        public void AddFromClone(GameObject gameObject, Transform parent, int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                AddToPool(Object.Instantiate(gameObject, parent));
            }
            
            Object.Destroy(gameObject);
        }

        private void AddToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
            
            goQueue.Enqueue(gameObject);
        }
    }
}
