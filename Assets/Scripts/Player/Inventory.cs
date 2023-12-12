using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;  //�κ��丮 Ű�� ���� ��.
    public Transform dropPosition;  //������ ��� ���õ�.

    [Header("Selected Item")]   //������ ���ÿ� ���õ� �ڵ�
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;    //�����۵� ����
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;    //��ư ����
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;  //�κ��丮 ����
    public UnityEvent onCloseInventory; //�κ��丮 �ݱ�.

    public static Inventory instance;   //�̱���
    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);  //�κ��丮 �޴��� ó������ ���Ѱ̴ϴ�.
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)  //�ݺ���. i�� ������ �κ��丮â�� ������, �� �ٽ� i�� ������ �κ��丮â�� ����. �ݺ�.
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear(); //UI �ʱ�ȭ
        }

        ClearSeletecItemWindow();
    }


    public void OnInventoryButton(InputAction.CallbackContext callbackContext)  //�κ��丮 �ѱ�.
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy) //Hieracrhy�� �����ֳ�, �ϰ� ����� ��.
        {
            inventoryWindow.SetActive(false);
            onCloseInventory?.Invoke(); //����.
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory?.Invoke();  //�ѱ�
            controller.ToggleCursor(true);  //�κ��丮�� ���� ���� Ŀ���� �� �� �ְ� �������.
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)  //EŰ�� ������ ��, ������ ó��.
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;   //���� �� �ִ� �Ŵ� ������ �״� �Ű�.(���� �������� ���� ���� ���, 2���� �������� ì��ٸ� ���� 2�� �߰� �ϱ� ���� ��.
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)  //������ ����ִ� �κ��丮�� ���� �Ű�.(��ĭ ã�ư��°Ű�)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }

        ThrowItem(item);
    }

    void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));  //�κ��丮�� �� á�� �� ����ض�.
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
                uiSlots[i].Set(slots[i]);   //���Կ� �ִ� �����ͷ� UI�� ��� �ֽ�ȭ�� �ɰ�����.
            else
                uiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item)    
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)   //Slots �������� ���� ã�� �ִ� �������̶�� slots[i].quantity�� item.maxStackAmount ���� ������
                return slots[i];                                                    //�������.
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;  //���ο� �����ִ� �����ͱ��� �����س�����.
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty; //������� �ۼ������� �������� �����ϴ� �ͱ��� ����.

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)  //�������� ����ϸ� ������ ü���̳� �׷� �͵��� ������.
        {
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        //��ư.
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);    //�������� �����ϰ� �ֳ�.
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);   //�������� ���� �� �ϰ��ֳ�.
        dropButton.SetActive(true); //�׻� �����־�� ��.
    }

    private void ClearSeletecItemWindow()   //Clear���ִ� �۾�
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()   //������ ��� ��ư
    {
        if (selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                }
            }
        }
        RemoveSelectedItem();   //����� �������� ����.
    }

    public void OnEquipButton()
    {

    }

    void UnEquip(int index)
    {

    }

    public void OnUnEquipButton()
    {

    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (uiSlots[selectedItemIndex].equipped)
            {
                UnEquip(selectedItemIndex); //���� ���̸�, UnEquip(��������) �ɾ��־�߰���.
            }

            selectedItem.item = null;
            ClearSeletecItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {

    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}
