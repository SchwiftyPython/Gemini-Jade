using System.Collections.Generic;
using Repos;
using UI.Orders;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class OrderLoader : MonoBehaviour
{
    [SerializeField] private AssetLabelReference orderAssetLabel;

    private void Awake()
    {
        LoadOrders();
    }

    public void AddOrder(OrderTemplate orderTemplate)
    {
        OrderRepo.orders.Add(orderTemplate);
    }

    public void LoadOrders()
    {
        OrderRepo.orders = new List<OrderTemplate>();

        Addressables.LoadAssetsAsync<OrderTemplate>(orderAssetLabel, (orderTemplate) =>
        {
            OrderRepo.orders.Add(orderTemplate);
            
            Debug.Log($"{orderTemplate.label} loaded");
        });
    }
}
