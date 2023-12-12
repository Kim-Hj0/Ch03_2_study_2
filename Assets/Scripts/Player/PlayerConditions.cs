using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public interface IDamagable //������ ó��.
{
    void TakePhysicalDamage(int damageAmount);
}

[System.Serializable]   //�غ�, ����Ƽ�� ���̰� ����.
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;  //�ִ밪
    public float startValue;  //���۰�.
    public float regenRate; //ȸ����
    public float decayRate; //�ر���
    public Image uiBar; //UI

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }
    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);  //0���ٴ� �������� �ʰ�.
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

    public float noHungerHealthDecay;   //������� �� ����� ���� ü���� �پ��.

    public UnityEvent onTakeDamage; //�������� �޾��� �� ó���� �̺�Ʈ

    // Start is called before the first frame update
    void Start()
    {
        health.curValue = health.startValue;    //���۰� ����.
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        stamina.Add(stamina.regenRate * Time.deltaTime); //���׹̳� �ֱ������� ȸ��

        if(hunger.curValue == 0.0f) 
            health.Subtract(noHungerHealthDecay * Time.deltaTime);  //������� ü���� ����.

        if (health.curValue == 0.0f)    //ü���� 0�� �Ǹ� ����.
            Die();

        health.uiBar.fillAmount = health.GetPercentage();   //����� UI���� ����.
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)  //ġ��
    {
        health.Add(amount);
    }

    public void Eat(float amount)   //�Դ�
    {
        hunger.Add(amount); //�����
    }

    public bool UseStamina(float amount)    //���׹̳� ������
    {
        if (stamina.curValue - amount < 0)  //�� �� �ִ��� üũ. �� �� ���ٸ� �Ⱦ�.
            return false;

        stamina.Subtract(amount);   //�� �� ������ ���.
        return true;
    }

    public void Die()
    {
        Debug.Log("�÷��̾ �׾���.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}
