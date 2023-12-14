using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;    //공격에 대한 속도
    private bool attacking;
    public float attackDistance;

    [Header("Resource Gathering")]  //자원을 채칩할 수 있는지
    public bool doesGatherResources;

    [Header("Combat")]  //전투를 진행할 수 있는지
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;   //카메라 가져오기.
        animator = GetComponent<Animator>();    //애니메이터 가져오기
    }

    public override void OnAttackInput()    //공격이 들어왔다면. 플레이어가 공격한다면.
    {
        if(!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);  //지연 실행.
        }
    }

    void OnCanAttack()  //지연 실행.
    {
        attacking = false;
    }


    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))  //실제로 쏘게 될거고.
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resouce))  //내가 공격하는 타이밍에 누군가가 있을텐데, 부딪힌 얘한테서 리소스 가져와.
            {
                resouce.Gather(hit.point, hit.normal);  //있으면 가져와달라.(아이템)
            }

            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damageable))  
            {
                damageable.TakePhysicalDamage(damage);
            }
        }
    }
}
