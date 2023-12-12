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

    public GameObject inventoryWindow;  //인벤토리 키고 끄는 거.
    public Transform dropPosition;  //아이템 드랍 관련된.

    [Header("Selected Item")]   //아이템 선택에 관련된 코드
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;    //아이템들 관련
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;    //버튼 관련
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;  //인벤토리 열기
    public UnityEvent onCloseInventory; //인벤토리 닫기.

    public static Inventory instance;   //싱글톤
    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);  //인벤토리 메뉴를 처음에는 꺼둘겁니다.
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)  //반복문. i를 누르면 인벤토리창이 켜지고, 또 다시 i를 누르면 인벤토리창이 꺼짐. 반복.
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear(); //UI 초기화
        }

        ClearSeletecItemWindow();
    }


    public void OnInventoryButton(InputAction.CallbackContext callbackContext)  //인벤토리 켜기.
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy) //Hieracrhy상에 켜져있냐, 하고 물어보는 것.
        {
            inventoryWindow.SetActive(false);
            onCloseInventory?.Invoke(); //끄기.
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory?.Invoke();  //켜기
            controller.ToggleCursor(true);  //인벤토리를 켰을 때만 커서를 쓸 수 있게 만들거임.
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)  //E키를 눌렀을 때, 아이템 처리.
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;   //모을 수 있는 거는 스택을 쌓는 거고.(같은 아이템이 여러 개일 경우, 2개의 아이템을 챙겼다면 숫자 2가 뜨게 하기 같은 거.
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)  //없으면 비어있는 인벤토리에 가는 거고.(빈칸 찾아가는거고)
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
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));  //인벤토리가 꽉 찼을 때 출력해라.
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
                uiSlots[i].Set(slots[i]);   //슬롯에 있는 데이터로 UI를 계속 최신화를 걸거있음.
            else
                uiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item)    
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)   //Slots 아이템이 지금 찾고 있는 아이템이라면 slots[i].quantity가 item.maxStackAmount 보다 작으면
                return slots[i];                                                    //돌려줘라.
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

        selectedItemName.text = selectedItem.item.displayName;  //내부에 숨어있는 데이터까지 접근해나가기.
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty; //여기까지 작성했으면 아이템을 습득하는 것까지 가능.

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)  //아이템을 사용하면 지정한 체력이나 그런 것들이 오른다.
        {
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        //버튼.
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped);    //아이템을 장착하고 있냐.
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);   //아이템을 장착 안 하고있냐.
        dropButton.SetActive(true); //항상 켜져있어야 함.
    }

    private void ClearSeletecItemWindow()   //Clear해주는 작업
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

    public void OnUseButton()   //아이템 사용 버튼
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
        RemoveSelectedItem();   //사용한 아이템은 삭제.
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
                UnEquip(selectedItemIndex); //장착 중이면, UnEquip(장착해제) 걸어주어야겠죠.
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
