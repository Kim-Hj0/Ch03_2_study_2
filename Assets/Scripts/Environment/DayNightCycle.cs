using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLength; //하루 루틴을 어느정도 사용할 건지.
    public float startTime = 0.4f; 
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;   //그라데이션을 어느정도.
    public AnimationCurve sunIntensity; //커브를 만들 수 있어

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
        time = (time + timeRate * Time.deltaTime) % 1.0f;   //%으로 쓸거니까 %f로.

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);   //환경광, 빛을 최소하시키려는 것.
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);  //반사광.
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);    //intensityCurve 시간에 맞는 시간값을 가져옴.

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;  //해가 뜨는 위치, 달이 뜨는 위치를 잡아줌.  //시간에 따라 빛 방향이 다르니까, 각도 변하는 거에 맞혀서 빛이 바뀜.
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
