using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; //맞았으면 줄 수 있는 아이템만 주면 된다. 나무를 때렸으니 장작 주기.
    public int quantityPerHit = 1;
    public int capacity;

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)    //생성하는 갯수만큼
        {
            if (capacity <= 0) { break; }
            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }

        if(capacity <= 0)
            Destroy(gameObject);
    }
 
}
