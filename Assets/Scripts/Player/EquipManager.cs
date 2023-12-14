using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController controller;
    private PlayerConditions conditions;

    // singleton
    public static EquipManager instance;
    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        conditions = GetComponent<PlayerConditions>();
    }

    public void OnAttackInput(InputAction.CallbackContext context)  //공격
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput(conditions);
        }
    }

    public void EquipNew(ItemData item)
    {
        UnEquip();
        curEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip != null)   //무언가 끼고있다는 소리죠.
        {
            Destroy(curEquip.gameObject);   //그럼 Destroy 합니다.
            curEquip = null;
        }
    }
}
