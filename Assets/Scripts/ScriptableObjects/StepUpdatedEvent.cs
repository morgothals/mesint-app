using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/StepUpdatedEvent")]
public class StepUpdatedEvent : ScriptableObject
{
    public event Action<int> OnEvent;
    public void Raise(int step) => OnEvent?.Invoke(step);
}