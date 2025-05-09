using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/GridChangedEvent")]
public class GridChangedEvent : ScriptableObject
{
    public event Action OnEvent;
    public void Raise() => OnEvent?.Invoke();
}