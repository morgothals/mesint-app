using UnityEngine;
using UnityEngine.UIElements;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    void OnEnable()
    {
        // 1) Lekérjük a gyökér elemet
        var root = uiDocument.rootVisualElement;

        // 2) Kiválasztjuk az 'ExitButton'-t (name="ExitButton")
        var exitBtn = root.Q<Button>("ExitButton");
        if (exitBtn == null)
        {
            Debug.LogError("ExitButton nem található a UI-ban!");
            return;
        }

        // 3) Feliratkozunk a clicked eseményre
        exitBtn.clicked += OnExitClicked;
    }

    void OnDisable()
    {
        var exitBtn = uiDocument.rootVisualElement.Q<Button>("ExitButton");
        if (exitBtn != null)
            exitBtn.clicked -= OnExitClicked;
    }

    private void OnExitClicked()
    {
        Debug.Log("Kilépés a játékból…");
        Application.Quit();
    }
}
