using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public EventSO Event;
    public UnityEvent Response;

    void OnEnable()
    {
        Event.Subscribe(this);
    }

    void OnDisable()
    {
        Event.UnSuscribe(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}