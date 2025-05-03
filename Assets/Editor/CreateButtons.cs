using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateButtons : EditorWindow
{
    [MenuItem("Window/Mesint/Custom Button")]
    public static void ShowWindow()
    {
        GetWindow<CreateButtons>("Custom Button");
    }

    public void CreateGUI()
    {
        // 1) Töltsd be a VisualTreeAsset-et
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/CreateButton.uxml");
        visualTree.CloneTree(rootVisualElement);

        // 2) Adj hozzá stíluslapot
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/Button.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        // 3) Kösd be a click eseményt
        var myButton = rootVisualElement.Q<Button>("myButton");
        myButton.clicked += () =>
        {
            Debug.Log("Editor gomb lenyomva!");
            // ide jöhet az Editor-specifikus logika
        };
    }
}