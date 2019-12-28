using Optimization.Caching;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class StyledComboBox : StyledItem
{
    private bool isToggled;

    [HideInInspector]
    [SerializeField]
    private List<StyledItem> items = new List<StyledItem>();

    [SerializeField]
    [HideInInspector]
    private StyledComboBoxPrefab root;

    [SerializeField]
    private int selectedIndex;

    public StyledComboBoxPrefab containerPrefab;
    public StyledItem itemMenuPrefab;
    public StyledItem itemPrefab;
    public StyledComboBox.SelectionChangedHandler OnSelectionChanged;

    public delegate void SelectionChangedHandler(StyledItem item);

    public int SelectedIndex
    {
        get
        {
            return this.selectedIndex;
        }
        set
        {
            if (value >= 0 && value <= this.items.Count)
            {
                this.selectedIndex = value;
                this.CreateMenuButton(this.items[this.selectedIndex].GetText().text);
            }
        }
    }

    public StyledItem SelectedItem
    {
        get
        {
            if (this.selectedIndex >= 0 && this.selectedIndex <= this.items.Count)
            {
                return this.items[this.selectedIndex];
            }
            return null;
        }
    }

    private void AddItem(object data)
    {
        if (this.itemPrefab != null)
        {
            Vector3[] array = new Vector3[4];
            this.itemPrefab.GetComponent<RectTransform>().GetLocalCorners(array);
            Vector3 position = array[0];
            float num = position.y - array[2].y;
            position.y = (float)this.items.Count * num;
            StyledItem styledItem = UnityEngine.Object.Instantiate(this.itemPrefab, position, this.root.itemRoot.rotation) as StyledItem;
            RectTransform component = styledItem.GetComponent<RectTransform>();
            styledItem.Populate(data);
            component.SetParent(this.root.itemRoot.transform, false);
            component.pivot = new Vector2(0f, 1f);
            component.anchorMin = new Vector2(0f, 1f);
            component.anchorMax = Vectors.v2one;
            component.anchoredPosition = new Vector2(0f, position.y);
            this.items.Add(styledItem);
            component.offsetMin = new Vector2(0f, position.y + num);
            component.offsetMax = new Vector2(0f, position.y);
            this.root.itemRoot.offsetMin = new Vector2(this.root.itemRoot.offsetMin.x, (float)(this.items.Count + 2) * num);
            Button button = styledItem.GetButton();
            int curIndex = this.items.Count - 1;
            if (button != null)
            {
                button.onClick.AddListener(delegate
                {
                    this.OnItemClicked(styledItem, curIndex);
                });
            }
        }
    }

    private void Awake()
    {
        this.InitControl();
    }

    private void CreateMenuButton(object data)
    {
        if (this.root.menuItem.transform.childCount > 0)
        {
            for (int i = this.root.menuItem.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyObject(this.root.menuItem.transform.GetChild(i).gameObject);
            }
        }
        if (this.itemMenuPrefab != null && this.root.menuItem != null)
        {
            StyledItem styledItem = UnityEngine.Object.Instantiate(this.itemMenuPrefab) as StyledItem;
            styledItem.Populate(data);
            styledItem.transform.SetParent(this.root.menuItem.transform, false);
            RectTransform component = styledItem.GetComponent<RectTransform>();
            component.pivot = new Vector2(0.5f, 0.5f);
            component.anchorMin = Vectors.v2zero;
            component.anchorMax = Vectors.v2one;
            component.offsetMin = Vectors.v2zero;
            component.offsetMax = Vectors.v2zero;
            this.root.gameObject.hideFlags = HideFlags.HideInHierarchy;
            Button button = styledItem.GetButton();
            if (button != null)
            {
                button.onClick.AddListener(new UnityAction(this.TogglePanelState));
            }
        }
    }

    public void AddItems(params object[] list)
    {
        this.ClearItems();
        for (int i = 0; i < list.Length; i++)
        {
            this.AddItem(list[i]);
        }
        this.SelectedIndex = 0;
    }

    public void ClearItems()
    {
        for (int i = this.items.Count - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyObject(this.items[i].gameObject);
        }
    }

    public void InitControl()
    {
        if (this.root != null)
        {
            UnityEngine.Object.DestroyImmediate(this.root.gameObject);
        }
        if (this.containerPrefab != null)
        {
            RectTransform component = base.GetComponent<RectTransform>();
            this.root = (UnityEngine.Object.Instantiate(this.containerPrefab, component.position, component.rotation) as StyledComboBoxPrefab);
            this.root.transform.SetParent(base.transform, false);
            RectTransform component2 = this.root.GetComponent<RectTransform>();
            component2.pivot = new Vector2(0.5f, 0.5f);
            component2.anchorMin = Vectors.v2zero;
            component2.anchorMax = Vectors.v2one;
            component2.offsetMax = Vectors.v2zero;
            component2.offsetMin = Vectors.v2zero;
            this.root.gameObject.hideFlags = HideFlags.HideInHierarchy;
            this.root.itemPanel.gameObject.SetActive(this.isToggled);
        }
    }

    public void OnItemClicked(StyledItem item, int index)
    {
        this.SelectedIndex = index;
        this.TogglePanelState();
        if (this.OnSelectionChanged != null)
        {
            this.OnSelectionChanged(item);
        }
    }

    public void TogglePanelState()
    {
        this.isToggled = !this.isToggled;
        this.root.itemPanel.gameObject.SetActive(this.isToggled);
    }
}