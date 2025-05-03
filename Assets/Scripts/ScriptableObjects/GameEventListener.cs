using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("A channel, amin keresztül hallgatunk az eseményre.")]
    public GameEvent EventChannel;

    [Tooltip("A response, amit meghívunk, amikor az esemény felérkezik.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        if (EventChannel != null)
            EventChannel.OnEventRaised.AddListener(OnEventRaised);
    }

    private void OnDisable()
    {
        if (EventChannel != null)
            EventChannel.OnEventRaised.RemoveListener(OnEventRaised);
    }

    private void OnEventRaised()
    {
        Response?.Invoke();
    }
}
