using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SettingsEventController : MonoBehaviour
{
    [Tooltip("Ide húzd be a SolverController GameObjectet!")]
    [SerializeField] private MonoBehaviour targetController;

    private UIDocument _uiDoc;
    private VisualElement _root;

    // mezőnév -> FieldInfo
    private FieldInfo[] _fields;

    void Awake()
    {
        if (targetController == null)
            throw new InvalidOperationException("Target Controller nincs beállítva!");

        _uiDoc = GetComponent<UIDocument>();
        _root = _uiDoc.rootVisualElement;

        // Lekérjük az összes public vagy [SerializeField] mezőt
        var t = targetController.GetType();
        _fields = t
          .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
          .ToArray();
    }

    void OnEnable()
    {
        foreach (var fi in _fields)
        {
            // keressünk UIElementet a név alapján
            var ve = _root.Q<VisualElement>(fi.Name);
            if (ve == null) continue;

            // get initial value
            object current = fi.GetValue(targetController);

            // EnumField
            if (ve is EnumField enumField && fi.FieldType.IsEnum)
            {
                // 1) kastoljuk az aktuális értéket Enum–re:
                var enumValue = (Enum)current;
                // 2) ezzel inicializáljuk:
                enumField.Init(enumValue);
                // 3) a callbacknél pedig már közvetlenül az Enum-et adjuk vissza:
                enumField.RegisterValueChangedCallback((ChangeEvent<Enum> evt) =>
                {
                    fi.SetValue(targetController, evt.newValue);
                });
            }
            // Toggle (bool)
            else if (ve is Toggle toggle && fi.FieldType == typeof(bool))
            {
                toggle.value = (bool)current;
                toggle.RegisterValueChangedCallback(evt =>
                {
                    fi.SetValue(targetController, evt.newValue);
                });
            }
            // IntegerField (int)
            else if (ve is IntegerField ifield && fi.FieldType == typeof(int))
            {
                ifield.value = (int)current;
                ifield.RegisterValueChangedCallback(evt =>
                {
                    fi.SetValue(targetController, evt.newValue);
                });
            }
            // FloatField (float)
            else if (ve is FloatField ffield && fi.FieldType == typeof(float))
            {
                ffield.value = (float)current;
                ffield.RegisterValueChangedCallback(evt =>
                {
                    fi.SetValue(targetController, evt.newValue);
                });
            }
            // TextField (string)
            else if (ve is TextField tfield && fi.FieldType == typeof(string))
            {
                tfield.value = (string)current;
                tfield.RegisterValueChangedCallback(evt =>
                {
                    fi.SetValue(targetController, evt.newValue);
                });
            }
            // Bővíthető: ha később új mezőtípus kell, itt hozzáadod 
        }
    }
}
