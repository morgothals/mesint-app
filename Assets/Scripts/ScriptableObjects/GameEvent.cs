using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    [Tooltip("Azok a callback-ek, amiket meghívunk, amikor Raise() fut.")]
    public UnityEvent OnEventRaised;

    /// <summary>
    /// Ezt hívd meg, amikor kibocsátod az eseményt.
    /// </summary>
    public void Raise()
    {
        OnEventRaised?.Invoke();
    }
}