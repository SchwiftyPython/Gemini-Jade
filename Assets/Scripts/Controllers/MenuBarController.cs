using System.Collections.Generic;
using Repos;
using TMPro;
using UI.Grid;
using UI.Orders;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using World.Things.CraftableThings;

namespace Controllers
{
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

        public SelectGridObjectButton gridObjectButtonPrefab; 

        public GameObject tabButtonsPrefab;
    
        public Transform tabParent;
    
        public Transform parentMenu;
    
        public MenuButton[] tabs;
    
        public MenuTab[] tabButtonPanels;
    
        public Color activeColor;
    
        public Color defaultButtonColor;

        public Color defaultTabColor;
    
        public int currentTabId = -1;
    
        public OrderTemplate currentOrder;

        public PlacedObjectTemplate currentPlacedObject;
    
        public Dictionary<string, MenuTabLink> links = new Dictionary<string, MenuTabLink>();
    
        public Dictionary<KeyCode, int> tabShortcuts = new Dictionary<KeyCode, int>();
    
        public Dictionary<KeyCode, OrderButton> keyboardShortcuts = new Dictionary<KeyCode, OrderButton>();

        public enum Tab
        {
            Orders,
            Zones,
            Buildings,
            Production,
            Furniture,
            Defense
        }
    
        private void Update()
        {
            if (currentOrder == null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) 
                {
                    Reset();
                }
            }

            foreach (var shortcut in tabShortcuts)
            {
                if (Input.GetKeyDown(shortcut.Key))
                {
                    ClickTab(shortcut.Value);
                }
            }

            //todo use enums for tab id -- also it'd make sense for us to have a base class for order button and placed object button
            //that way we can just define a <TabId, <KeyCode, BaseClass Button>>
            if (currentTabId == 0)
            {
                foreach (var shortcut in keyboardShortcuts)
                {
                    if (Input.GetKeyDown(shortcut.Key))
                    {
                        ClickOrder(shortcut.Value);
                    }
                }
            }
        }

        public void Setup()
        {
            //if we end up with a lot of tabs, especially if we have to add sub tabs, should consider scriptable objects
            //it would be easier to define keycodes and whatnot
            
            var tabCount = CollectionUtils.EnumToArray<Tab>().Length;

            tabs = new MenuButton[tabCount];

            tabButtonPanels = new MenuTab[tabCount];

            AddTab(Tab.Orders, KeyCode.O);

            //Placeholder tabs TBD

            AddTab(Tab.Zones, KeyCode.Z);

            AddTab(Tab.Buildings, KeyCode.B);

            AddTab(Tab.Production, KeyCode.P);

            AddTab(Tab.Furniture, KeyCode.F);

            AddTab(Tab.Defense, KeyCode.L);

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

            currentTabId = -1;
        
            ClearSelection();
        }

        public void AddTab(Tab tabToAdd, KeyCode shortcut)
        {
            TextMeshProUGUI text;

            Image image = null;

            Button button;

            var id = (int)tabToAdd;

            var tabButtons = Instantiate(tabButtonsPrefab, parentMenu);

            tabButtons.name = $"TabButtons: {tabToAdd}";

            tabButtonPanels[id] = new MenuTab(tabButtons);

            //todo also other tabs aren't going to have order templates. Placed objects use a placed object template.
            //probably going to have to use some baseclass. Not really sure yet.
            //My concern is with modding they won't be able to add tabs easily.
            //It will be easier to see once we start adding the wall button

            if (tabToAdd == Tab.Orders)
            {
                PopulateOrders(tabButtons.transform);
            }
            else if (tabToAdd == Tab.Buildings)
            {
                PopulateBuildings(tabButtons.transform);
            }

            var tab = Instantiate(tabPrefab, tabParent);

            tab.name = $"Tab: {tabToAdd}";

            text = tab.GetComponentInChildren<TextMeshProUGUI>();

            text.text = tabToAdd.ToString();

            tabShortcuts.Add(shortcut, id);

            text.text += $" ({shortcut})";
        
            button = tab.GetComponent<Button>();
        
            button.onClick.AddListener(delegate { ClickTab(id); });

            image = tab.GetComponentInChildren<Image>();

            tabs[id] = new MenuButton(tab, button, text, image);
        }

        public void ClickOrder(OrderButton orderButton)
        {
            ClearOrders();

            if (currentOrder != orderButton.orderTemplate)
            {
                links[orderButton.orderTemplate.templateName].image.color = activeColor;
         
                orderButton.OnClick();
            }
            else
            {
                currentOrder = null;
            }
        }

        public void ClickTab(int id)
        {
            if (currentTabId != id)
            {
                ClearSelection();

                currentTabId = id;

                tabs[currentTabId].image.color = activeColor;
            
                tabButtonPanels[currentTabId].go.SetActive(true);
            }
            else
            {
                Reset();
            }
        }

        public void ClickGridObject(SelectGridObjectButton gridObjectButton)
        {
            if (currentPlacedObject != gridObjectButton.placedObjectType)
            {
                gridObjectButton.HandleButtonClicked();
            }
            else
            {
                currentPlacedObject = null;
            }
        }

        private void PopulateOrders(Transform parent)
        {
            var orders = new List<OrderTemplate>(OrderRepo.orders);

            foreach (var order in orders)
            {
                var orderButton = Instantiate(orderButtonPrefab, parent);

                orderButton.name = $"OrderButton: {order.label}";

                orderButton.orderTemplate = order;

                var textFields = orderButton.GetComponentsInChildren<TextMeshProUGUI>();

                var text = textFields[0];

                text.text = order.LabelCap;

                text = textFields[1];

                text.text = order.keyboardShortcut.ToString();

                var image = orderButton.GetComponentsInChildren<Image>()[1];

                image.sprite = Sprite.Create(order.graphics.texture,
                    new Rect(0, 0, order.graphics.texture.width, order.graphics.texture.height),
                    new Vector2(0.5f, 0.5f));

                keyboardShortcuts.Add(order.keyboardShortcut, orderButton);

                var link = new MenuTabLink(orderButton.gameObject, image);

                links.Add(order.templateName, link);

                var button = orderButton.GetComponent<Button>();

                button.onClick.AddListener(delegate { ClickOrder(orderButton); });
            }
        }

        private void PopulateBuildings(Transform parent)
        {
            var buildingRepo = FindObjectOfType<BuildablesRepo>();
            
            var buildings = new List<PlacedObjectTemplate>(buildingRepo.buildings);

            foreach (var building in buildings)
            {
                var buildingButton = Instantiate(gridObjectButtonPrefab, parent);

                buildingButton.name = $"Building Button: {building.label}";

                buildingButton.placedObjectType = building;

                var label = buildingButton.GetComponentInChildren<TextMeshProUGUI>();

                label.text = building.LabelCap;
                
                var image = buildingButton.GetComponentsInChildren<Image>()[1];
                
                //todo when we switch walls to a mesh
                // image.sprite = Sprite.Create(building.graphics.texture,
                //     new Rect(0, 0, building.graphics.texture.width, building.graphics.texture.height),
                //     new Vector2(0.5f, 0.5f));

                image.sprite = building.builtTexture;

                var button = buildingButton.GetComponent<Button>();
                
                button.onClick.AddListener(delegate { ClickGridObject(buildingButton); });
            }
        }
    }
}