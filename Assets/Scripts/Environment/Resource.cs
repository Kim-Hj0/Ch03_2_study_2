using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; //�¾����� �� �� �ִ� �����۸� �ָ� �ȴ�. ������ �������� ���� �ֱ�.
    public int quantityPerHit = 1;
    public int capacity;

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)    //�����ϴ� ������ŭ
        {
            if (capacity <= 0) { break; }
            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }

        if(capacity <= 0)
            Destroy(gameObject);
    }
 
}
