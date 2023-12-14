using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;    //���ݿ� ���� �ӵ�
    private bool attacking;
    public float attackDistance;

    [Header("Resource Gathering")]  //�ڿ��� äĨ�� �� �ִ���
    public bool doesGatherResources;

    [Header("Combat")]  //������ ������ �� �ִ���
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;   //ī�޶� ��������.
        animator = GetComponent<Animator>();    //�ִϸ����� ��������
    }

    public override void OnAttackInput()    //������ ���Դٸ�. �÷��̾ �����Ѵٸ�.
    {
        if(!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);  //���� ����.
        }
    }

    void OnCanAttack()  //���� ����.
    {
        attacking = false;
    }


}
