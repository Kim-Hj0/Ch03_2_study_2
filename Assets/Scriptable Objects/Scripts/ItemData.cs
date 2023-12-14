using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,   //리소스로 사용할 거.
    Equipable,  //장착할 거.
    Consumable  //소모할 거.
}

public enum ConsumableType  //소모되면서 적용될 컨디션이 무엇인지.
{
    Hunger,
    Health
}

[System.Serializable]   //유니티에 나타나게 하기.
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")] //어떤 아이템 데이터에서 어떤 타입의 능력치를 줄 것인가 설정. 
    public ItemDataConsumable[] consumables;

    [Header("Equip")]   //무기
    public GameObject equipPrefab;


}



