using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength; //�Ϸ� ��ƾ�� ������� ����� ����.
    public float startTime = 0.4f; 
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;   //�׶��̼��� �������.
    public AnimationCurve sunIntensity; //Ŀ�긦 ���� �� �־�

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;


    // Start is called before the first frame update
    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    // Update is called once per frame
    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;   //%���� ���Ŵϱ� %f��.

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);   //ȯ�汤, ���� �ּ��Ͻ�Ű���� ��.
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);  //�ݻ籤.
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);    //intensityCurve �ð��� �´� �ð����� ������.

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;  //�ذ� �ߴ� ��ġ, ���� �ߴ� ��ġ�� �����.  //�ð��� ���� �� ������ �ٸ��ϱ�, ���� ���ϴ� �ſ� ������ ���� �ٲ�.
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
