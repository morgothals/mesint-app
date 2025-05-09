
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/MessageEvent")]
public class MessageEvent : ScriptableObject
{
    public event Action<string> OnEvent;
    public void Raise(string msg) => OnEvent?.Invoke(msg);
}