using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class NaiveSolver1ButtonController : MonoBehaviour
{


    public GameEvent CreatePressedEvent;
    private Button myButton;

    void Awake()
    {
        var doc = GetComponent<UIDocument>();
        // Ha nem a UIDocumentben töltöd, mint ebben az esetben, 
        // akkor CloneTree-t kell hívni:
        // var root = doc.visualTreeAsset.CloneTree();
        // doc.rootVisualElement.Add(root);

        var root = doc.rootVisualElement;
        myButton = root.Q<Button>("Solver_1_Button");
        myButton.clicked += () => CreatePressedEvent?.Raise();
    }


    void OnDestroy()
    {
        myButton.clicked -= () => CreatePressedEvent?.Raise();
    }
}
