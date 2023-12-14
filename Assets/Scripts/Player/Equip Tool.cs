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


    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))  //������ ��� �ɰŰ�.
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resouce))  //���� �����ϴ� Ÿ�ֿ̹� �������� �����ٵ�, �ε��� �����׼� ���ҽ� ������.
            {
                resouce.Gather(hit.point, hit.normal);  //������ �����ʹ޶�.(������)
            }

            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damageable))  
            {
                damageable.TakePhysicalDamage(damage);
            }
        }
    }
}
