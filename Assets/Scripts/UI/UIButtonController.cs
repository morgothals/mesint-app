using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIButtonsController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Game Refs")]
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private CubeMover cubeMover;
    [SerializeField] private SolverController solverController;

    private Button exitButton;
    private Button pauseButton;
    private Button stopButton;
    private Button createBoardButton;
    private Button playButton;

    private bool isPaused = false;

    private Color defaultCreateColor;
    private Color defaultPauseColor;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        // 1) Lekérjük a gombokat
        exitButton = root.Q<Button>("ExitButton");
        pauseButton = root.Q<Button>("PauseButton");
        stopButton = root.Q<Button>("StopButton");
        createBoardButton = root.Q<Button>("CreateBoardButton");
        playButton = root.Q<Button>("PlayButton");

        if (createBoardButton != null)
            defaultCreateColor = createBoardButton.resolvedStyle.backgroundColor;
        if (pauseButton != null)
            defaultPauseColor = pauseButton.resolvedStyle.backgroundColor;

        // 2) Feliratkozunk a clicked eseményekre
        if (exitButton != null) exitButton.clicked += OnExitClicked;
        if (pauseButton != null) pauseButton.clicked += OnPauseClicked;
        if (stopButton != null) stopButton.clicked += OnStopClicked;
        if (createBoardButton != null) createBoardButton.clicked += OnCreateClicked;
        if (playButton != null) playButton.clicked += OnPlayClicked;

        if (solverController != null)
            solverController.OnSolveCompleted += RestoreCreateButton;
    }

    private void OnCreateClicked()
    {
        gridSpawner.Initialize();
    }

    void OnDisable()
    {
        if (exitButton != null) exitButton.clicked -= OnExitClicked;
        if (pauseButton != null) pauseButton.clicked -= OnPauseClicked;
        if (stopButton != null) stopButton.clicked -= OnStopClicked;
        if (playButton != null) playButton.clicked -= OnPlayClicked;

        if (solverController != null)
            solverController.OnSolveCompleted -= RestoreCreateButton;
    }

    private void RestoreCreateButton()
    {
        if (createBoardButton != null)
        {
            createBoardButton.SetEnabled(true);
            createBoardButton.style.opacity = new StyleFloat(1f);
        }
    }

    private void OnExitClicked()
    {
        Debug.Log("Kilépés a játékból…");
        Application.Quit();
    }

    private void OnPauseClicked()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;



        if (createBoardButton != null)
        {
            createBoardButton.SetEnabled(!isPaused);
            createBoardButton.style.opacity = new StyleFloat(isPaused ? 0.3f : 1f);
        }

        if (playButton != null)
        {
            playButton.SetEnabled(!isPaused);
            playButton.style.opacity = new StyleFloat(isPaused ? 0.3f : 1f);
        }

        if (stopButton != null)
        {
            stopButton.SetEnabled(!isPaused);
            stopButton.style.opacity = new StyleFloat(isPaused ? 0.3f : 1f);
        }

        Debug.Log(isPaused ? "Játék szünetelve" : "Játék folytatódik");
    }

    private void OnPlayClicked()
    {
        // disable + 30% opacity
        if (createBoardButton != null)
        {
            createBoardButton.SetEnabled(false);
            createBoardButton.style.opacity = new StyleFloat(0.3f);
        }

        solverController.HandleSolveRequest();

        StartCoroutine(MonitorRunEnd());
    }

    private IEnumerator MonitorRunEnd()
    {
        // várunk, míg a CubeMover beállítja az IsMoving-et
        yield return new WaitUntil(() => cubeMover.IsMoving);
        // most holtrovat vége: megvárjuk, amíg végez
        yield return new WaitUntil(() => !cubeMover.IsMoving);

        // visszaállítjuk a Create gombot
        if (createBoardButton != null)
        {
            createBoardButton.SetEnabled(true);
            createBoardButton.style.opacity = new StyleFloat(1f);
        }
    }

    private void OnStopClicked()
    {
        // idő restore
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseButton != null)
            pauseButton.style.opacity = new StyleFloat(1f);

        // csak kocka reset
        cubeMover?.ResetCube();
        Debug.Log("Kocka visszaállítva.");
    }
}
