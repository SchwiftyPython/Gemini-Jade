using System;
using System.Collections;
using System.Collections.Generic;
using Repos;
using UI.Orders;
using UnityEngine;
using UnityEngine.UI;

public struct MenuTab
{
    public GameObject go;

    public MenuTab(GameObject go)
    {
        this.go = go;
    }
}

public struct MenuTabLink
{
    public GameObject go;
    public Image image;

    public MenuTabLink(GameObject go, Image image)
    {
        this.go = go;
        this.image = image;
    }
}

public struct MenuButton
{
    public GameObject go;
    public Button button;
    public Text text;
    public Image image;

    public MenuButton(GameObject go, Button button, Text text, Image image)
    {
        this.go = go;
        this.button = button;
        this.text = text;
        this.image = image;
    }
}

public class MenuBarController : MonoBehaviour
{
    public GameObject tabPrefab;

    public OrderButton orderButtonPrefab;
    
    public Transform parent;
    
    public Transform parentMenu;
    
    public MenuButton[] buttons;
    
    public MenuTab[] tabs;
    
    public Color activeColor;
    
    public Color defaultColor;
    
    public int current = -1;
    
    public OrderTemplate currentOrder;
    
    public Dictionary<string, MenuTabLink> links = new Dictionary<string, MenuTabLink>();
    
    public Dictionary<KeyCode, int> tabShortcuts = new Dictionary<KeyCode, int>();
    
    public Dictionary<KeyCode, OrderTemplate> keyboardShortcuts = new Dictionary<KeyCode, OrderTemplate>();
    
    private void Start()
    {
        const int tabCount = 1;
    }

    public void ClearSelection()
    {
        foreach (var button in buttons)
        {
            button.image.color = defaultColor;
        }

        foreach (var tab in tabs)
        {
            tab.go.SetActive(false);
        }
        
        //todo reset tooltip
    }

    public void ClearOrders()
    {
        foreach (var link in links.Values)
        {
            link.image.color = defaultColor;
        }
    }

    public void Reset()
    {
        currentOrder = null;
        
        ClearOrders();

        current = -1;
        
        ClearSelection();
    }

    public void AddTab(string tabName, int id, KeyCode shortcut)
    {
        Text text;

        Image image;

        Button button;

        var tab = Instantiate(tabPrefab);
        
        tab.transform.SetParent(parent);

        tab.name = $"Tab: {tabName}";

        tabs[id] = new MenuTab(tab);

        var orders = new List<OrderTemplate>();
        
        //todo probably use enum for id so a little easier to read
        
        //todo also other tabs aren't going to have order templates. Placed objects use a placed object template.
        //probably going to have to use some baseclass. Not really sure yet.
        //My concern is with modding they won't be able to add tabs easily.

        if (id == 0)
        {
            orders = new List<OrderTemplate>(OrderRepo.orders);
        }

        foreach (var order in orders)
        {
            var orderButton = Instantiate(orderButtonPrefab);
            
            orderButton.transform.SetParent(tab.transform);
            
            
        }
    }
}