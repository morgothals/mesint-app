
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/SolverStartedEvent")]
public class SolverStartedEvent : ScriptableObject
{
    public event Action<string> OnEvent;
    public void Raise(string solverName) => OnEvent?.Invoke(solverName);
}

