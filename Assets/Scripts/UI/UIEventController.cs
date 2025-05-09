using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIEventController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Events")]
    [SerializeField] private SolverStartedEvent solverStartedEvent;
    [SerializeField] private StepUpdatedEvent stepUpdatedEvent;
    [SerializeField] private MessageEvent messageEvent;

    // runtime-ban tölti be
    private Label infoLabel;
    private Label stepsLabel;
    private Label messageLabel;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // Megkeressük a két Label-t
        infoLabel = root.Q<Label>("Info_Label");
        stepsLabel = root.Q<Label>("Steps_Label");
        messageLabel = root.Q<Label>("warningMessage");
        messageLabel.text = "";

        // Feliratkozunk az eseményekre
        solverStartedEvent.OnEvent += OnSolverStarted;
        stepUpdatedEvent.OnEvent += OnStepUpdated;
        messageEvent.OnEvent += OnMessageSent;
    }

    void OnDisable()
    {
        solverStartedEvent.OnEvent -= OnSolverStarted;
        stepUpdatedEvent.OnEvent -= OnStepUpdated;
        messageEvent.OnEvent -= OnMessageSent;
    }

    private void OnMessageSent(string message)
    {
        if (messageLabel != null)
        {
            messageLabel.text = message;
        }
    }

    private void OnSolverStarted(string solverName)
    {
        if (infoLabel != null)
            infoLabel.text = solverName;
    }

    private void OnStepUpdated(int step)
    {
        if (stepsLabel != null)
            stepsLabel.text = step.ToString();
    }
}
