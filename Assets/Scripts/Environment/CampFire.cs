using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;  //������
    public float damageRate;    //�پ�� ������

    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate);   //���� �������� �� �� �Ұų�. Invoke���� ����. Repeating �̸�ŭ �ض�. 0 �ٷ� ����.
    }

    void DealDamage()
    {
        for(int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);   //�ɸ��� ����� �� ������ �ߵ�.
        }
    }

    private void OnTriggerEnter(Collider other) //���ݴ��ϴ� ���.
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
