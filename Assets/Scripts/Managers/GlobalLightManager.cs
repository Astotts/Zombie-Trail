using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight2D;
    [SerializeField] float dayIntensify;
    [SerializeField] float nightIntensify;

    void Start()
    {
        WaveManager.Instance.OnStateChange += OnWaveStateChange;
    }

    void OnWaveStateChange(object sender, WaveState state)
    {
        if (state == WaveState.StartDay)
        {
            globalLight2D.intensity = dayIntensify;
        }
        else if (state == WaveState.StartNight)
        {
            globalLight2D.intensity = nightIntensify;
        }
    }
}