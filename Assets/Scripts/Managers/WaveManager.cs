using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WaveManager : NetworkBehaviour
{
    public static WaveManager Instance { get; private set; }
    public int CurrentDay { get; private set; }

    public event EventHandler<WaveState> OnStateChange;

    [SerializeField] WaveManagerStats stats;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text subTitle;

    readonly NetworkVariable<WaveState> currentState = new();

    DateTime currentTime;
    Coroutine dayCoroutine;

    bool isDay;

    string skipKey;
    string underlinedSkipKey;

    void Awake()
    {
        if (Instance != null)
            Debug.LogError("There are more than one WaveManager Instance!");
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
            OnStateChange?.Invoke(this, currentState.Value);

        currentState.OnValueChanged += OnCurrentStateChange;

        base.OnNetworkSpawn();
    }

    void Start()
    {
        if (!IsServer)
            return;

        StartNight();

        skipKey = stats.SkipKey;
        underlinedSkipKey = "<u>" + skipKey + "</u>";
    }
    void StartNight()
    {
        if (isDay)
            StopCoroutine(dayCoroutine);

        StartCoroutine(NightTimer());
    }

    IEnumerator NightTimer()
    {
        currentTime = new DateTime().AddHours(stats.StartTime.Hour).AddMinutes(stats.StartTime.Minute);
        ChangeState(WaveState.StartNight);

        int nightTime = stats.SecondsPerNight;
        for (int i = 0; i < nightTime; i++)
        {
            title.text = currentTime.AddMinutes(i).ToString("hh:mm tt");
            yield return new WaitForSeconds(1.0f);
        }

        title.text = "";
        ChangeState(WaveState.EndNight);

        StartDay();
    }

    void StartDay()
    {
        dayCoroutine = StartCoroutine(DayTimer());
    }

    IEnumerator DayTimer()
    {
        isDay = true;
        ChangeState(WaveState.StartDay);

        int dayTime = stats.SecondsPerDay;
        for (int i = dayTime; i > 0; i--)
        {
            string key = i % 2 == 0 ? skipKey : underlinedSkipKey;
            title.text = string.Format(stats.DayTitle, i.ToString());
            subTitle.text = string.Format(stats.DaySubtitle, key);
            yield return new WaitForSeconds(1.0f);
        }

        ChangeState(WaveState.EndDay);
        CurrentDay++;
        isDay = false;

        StartNight();
    }

    void ChangeState(WaveState waveState)
    {
        currentState.Value = waveState;
    }

    void OnCurrentStateChange(WaveState previous, WaveState current)
    {
        OnStateChange?.Invoke(this, current);
    }
}

public enum WaveState
{
    StartNight,
    EndNight,
    StartDay,
    EndDay
}
