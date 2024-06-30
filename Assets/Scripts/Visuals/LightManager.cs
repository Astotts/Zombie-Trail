using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    Light2D[] lights;

    public Light2D dayLight;

    void OnEnable()
    {   
        GameManager.OnStateChange += UpdateTime;
        lights = FindObjectsOfType<Light2D>();
    }

    void OnDisable()
    {
        GameManager.OnStateChange -= UpdateTime;
    }

    void UpdateTime(GameState state)
    {
        if (state == GameState.WaitStart){
            AllLightsOff();
        }
        else if (state == GameState.WaveStart)
            AllLightsOn();
    }

    public void AllLightsOn()
    {
        foreach (Light2D l in lights)
            l.enabled = true;
        dayLight.enabled = false;
    }

    public void AllLightsOff()
    {
        foreach (Light2D l in lights)
            l.enabled = false;
        dayLight.enabled = true;
    }
}
