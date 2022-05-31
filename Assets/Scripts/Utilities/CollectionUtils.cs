using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities
{
    public static class CollectionUtils
    {
        public static T RandomElementByWeight<T>(this IEnumerable<T> collection, Func<T, float> weightSelector)
        {
            var totalWeight = 0f;

            var list = collection as List<T>;

            if (list != null)
            {
                foreach (var element in list)
                {
                    var weight = weightSelector(element);

                    if (weight < 0f)
                    {
                        Debug.LogError($"Negative weight in selector: {weight} from {element}");
                        weight = 0f;
                    }

                    totalWeight += weight;
                }

                if (list.Count == 1)
                {
                    if (totalWeight > 0f)
                    {
                        return list.First();
                    }
                }
            }
            else
            {
                var numElements = 0;

                foreach (var element in collection)
                {
                    numElements++;
                    
                    var weight = weightSelector(element);
                    
                    if (weight < 0f)
                    {
                        Debug.LogError($"Negative weight in selector: {weight} from {element}");
                        weight = 0f;
                    }

                    totalWeight += weight;
                }

                if (numElements == 1)
                {
                    if (totalWeight > 0f)
                    {
                        return collection.First();
                    }
                }
            }

            if (totalWeight <= 0f)
            {
                Debug.LogError($"RandomElementByWeight with total weight: {totalWeight} - Use TryRandomElementByWeight");
                return default;
            }

            var weightThreshold = Random.Range(0, 100) * totalWeight; //they use murmurhash here if this doesn't work

            var runningTotal = 0f;

            if (list != null)
            {
                foreach (var element in list)
                {
                    var weight = weightSelector(element);

                    if (!(weight > 0f))
                    {
                        continue;
                    }

                    runningTotal += weight;

                    if (runningTotal >= weightThreshold)
                    {
                        return element;
                    }
                }
            }
            else
            {
                foreach (var element in collection)
                {
                    var weight = weightSelector(element);

                    if (!(weight > 0f))
                    {
                        continue;
                    }

                    runningTotal += weight;

                    if (runningTotal >= weightThreshold)
                    {
                        return element;
                    }
                }
            }

            return default;
        }
    }
}
