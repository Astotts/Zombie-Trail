using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventSO : ScriptableObject
{
    private readonly List<GameEventListener> listeners = new();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void Subscribe(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnSuscribe(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}