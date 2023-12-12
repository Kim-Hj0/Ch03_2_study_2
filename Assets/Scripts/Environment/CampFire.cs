using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;  //데미지
    public float damageRate;    //줄어들 데미지

    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);   //언제 데미지가 들어갈 게 할거냐. Invoke지연 실행. Repeating 이만큼 해라. 0 바로 실행.
    }

    void DealDamage()
    {
        for(int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);   //걸리는 얘들은 다 데미지 발동.
        }
    }

    private void OnTriggerEnter(Collider other) //공격당하는 얘들.
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable);
        }
    }
}
