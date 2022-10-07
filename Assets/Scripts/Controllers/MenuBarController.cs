using System.Collections.Generic;
using Repos;
using TMPro;
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
    public TextMeshProUGUI text;
    public Image image;

    public MenuButton(GameObject go, Button button, TextMeshProUGUI text, Image image)
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

    public GameObject tabButtonsPrefab;
    
    public Transform tabParent;
    
    public Transform parentMenu;
    
    public MenuButton[] tabs;
    
    public MenuTab[] tabButtonPanels;
    
    public Color activeColor;
    
    public Color defaultButtonColor;

    public Color defaultTabColor;
    
    public int current = -1;
    
    public OrderTemplate currentOrder;
    
    public Dictionary<string, MenuTabLink> links = new Dictionary<string, MenuTabLink>();
    
    public Dictionary<KeyCode, int> tabShortcuts = new Dictionary<KeyCode, int>();
    
    public Dictionary<KeyCode, OrderTemplate> keyboardShortcuts = new Dictionary<KeyCode, OrderTemplate>();
    
    private void Start()
    {
        
    }

    public void Setup()
    {
        const int tabCount = 6;

        tabs = new MenuButton[tabCount];

        tabButtonPanels = new MenuTab[tabCount];
        
        AddTab("Orders", 0, KeyCode.O);
        
        //Placeholder tabs TBD
        
        AddTab("Zones", 1, KeyCode.Z);
        
        AddTab("Structures", 2, KeyCode.S);
        
        AddTab("Production", 2, KeyCode.P);
        
        AddTab("Magic", 3, KeyCode.M);
        
        AddTab("Combat", 4, KeyCode.C);

        currentOrder = null;
        
        Reset();
    }

    public void ClearSelection()
    {
        foreach (var tab in tabs)
        {
            tab.image.color = defaultTabColor;
        }

        foreach (var panel in tabButtonPanels)
        {
            panel.go.SetActive(false);
        }
        
        //todo reset tooltip
    }

    public void ClearOrders()
    {
        foreach (var link in links.Values)
        {
            link.image.color = defaultButtonColor;
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
        TextMeshProUGUI text;

        Image image = null;

        Button button;

        var tabButtons = Instantiate(tabButtonsPrefab, parentMenu);

        tabButtons.name = $"TabButtons: {tabName}";

        tabButtonPanels[id] = new MenuTab(tabButtons); 

        var orders = new List<OrderTemplate>();
        
        //todo probably use enum for id so a little easier to read
        
        //todo also other tabs aren't going to have order templates. Placed objects use a placed object template.
        //probably going to have to use some baseclass. Not really sure yet.
        //My concern is with modding they won't be able to add tabs easily.
        //It will be easier to see once we start adding the wall button

        if (id == 0)
        {
            orders = new List<OrderTemplate>(OrderRepo.orders);
        }
        
        //todo instantiate tab buttons prefab. Each tab has it's own group of tab buttons which are setup in the same spot just with their own set
        //of buttons. Might need a parent object to keep organized. Also, rename the parent transforms in this class because they are vague. Should have tab parent, tab buttons parent etc

        foreach (var order in orders)
        {
            var orderButton = Instantiate(orderButtonPrefab, tabButtons.transform);

            orderButton.name = $"OrderButton: {order.label}";

            var textFields = orderButton.GetComponentsInChildren<TextMeshProUGUI>();

            text = textFields[0];

            text.text = order.LabelCap;

            text = textFields[1];

            text.text = order.keyboardShortcut.ToString();

            image = orderButton.GetComponentsInChildren<Image>()[1];

            image.sprite = Sprite.Create(order.graphics.texture,
                new Rect(0, 0, order.graphics.texture.width, order.graphics.texture.height), new Vector2(0.5f, 0.5f));
            
            keyboardShortcuts.Add(order.keyboardShortcut, order);

            var link = new MenuTabLink(orderButton.gameObject, image);
            
            links.Add(order.templateName, link);
            
            button = orderButton.GetComponent<Button>();
            
            button.onClick.AddListener(delegate { orderButton.OnClick(); });
        }

        var tab = Instantiate(tabPrefab, tabParent);

        tab.name = $"Tab: {tabName}";

        text = tab.GetComponentInChildren<TextMeshProUGUI>();

        text.text = tabName;

        tabShortcuts.Add(shortcut, id);

        text.text += $" ({shortcut})";
        
        button = tab.GetComponent<Button>();
        
        button.onClick.AddListener(delegate { ClickTab(id); });

        image = tab.GetComponentInChildren<Image>();

        tabs[id] = new MenuButton(tab, button, text, image);
    }

    public void ClickTab(int id)
    {
        
    }
}