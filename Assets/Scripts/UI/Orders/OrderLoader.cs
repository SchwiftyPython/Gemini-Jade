using System.Collections.Generic;
using Repos;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Orders
{
    public class OrderLoader : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference orderAssetLabel;

        private static void AddOrder(OrderTemplate orderTemplate)
        {
            OrderRepo.orders.Add(orderTemplate);
        }

        public void LoadOrders()
        {
            OrderRepo.orders = new List<OrderTemplate>();

            Addressables.LoadAssetsAsync<OrderTemplate>(orderAssetLabel, orderTemplate =>
            {
                AddOrder(orderTemplate);
            
                Debug.Log($"{orderTemplate.label} loaded");
            });
        }
    }
}
