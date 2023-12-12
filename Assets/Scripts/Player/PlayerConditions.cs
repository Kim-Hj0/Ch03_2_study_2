using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public interface IDamagable //데미지 처리.
{
    void TakePhysicalDamage(int damageAmount);
}

[System.Serializable]   //준비, 유니티상에 보이게 해줌.
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;  //최대값
    public float startValue;  //시작값.
    public float regenRate; //회복률
    public float decayRate; //붕괴율
    public Image uiBar; //UI

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }
    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);  //0보다는 낮아지지 않게.
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }

}


public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    public float noHungerHealthDecay;   //배고픔이 다 닳았을 때는 체력이 줄어듬.

    public UnityEvent onTakeDamage; //데미지를 받았을 때 처리할 이벤트

    // Start is called before the first frame update
    void Start()
    {
        health.curValue = health.startValue;    //시작값 지정.
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        stamina.Add(stamina.regenRate * Time.deltaTime); //스테미나 주기적으로 회복

        if(hunger.curValue == 0.0f) 
            health.Subtract(noHungerHealthDecay * Time.deltaTime);  //배고프면 체력이 깍임.

        if (health.curValue == 0.0f)    //체력이 0이 되면 죽음.
            Die();

        health.uiBar.fillAmount = health.GetPercentage();   //사용할 UI까지 연결.
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)  //치유
    {
        health.Add(amount);
    }

    public void Eat(float amount)   //먹다
    {
        hunger.Add(amount); //배고픔
    }

    public bool UseStamina(float amount)    //스테미나 쓰려면
    {
        if (stamina.curValue - amount < 0)  //쓸 수 있는지 체크. 쓸 수 없다면 안씀.
            return false;

        stamina.Subtract(amount);   //쓸 수 있으면 사용.
        return true;
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}
