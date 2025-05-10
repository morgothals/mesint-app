using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private Transform cubeTransform;

    [Range(1, 60)]
    [SerializeField] private float movesPerSecond = 6f;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;

    [Header("Events")]
    [SerializeField] private StepUpdatedEvent stepUpdatedEvent;


    [Header("Step Marker")]
    [SerializeField] private GameObject stepMarkerPrefab;
    [SerializeField] private float markerHeightOffset = 0.5f;

    [Header("Fade Override (optional)")]
    [SerializeField] private bool overrideMarkerFade = false;
    [SerializeField] private bool defaultEnableFade = true;
    [SerializeField] private float defaultFadeDuration = 1f;

    private Queue<(Vector3 targetPosition, int redFaceIndex)> moveQueue = new();
    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private Transform[] faceQuads; // a 6 quad a kocka alá

    private readonly List<GameObject> spawnedMarkers = new();
    private Vector3 initialCubePos;


    void Start()
    {
        //Initialize();
    }

    void Initialize()
    {
        if (cubeTransform == null)
        {
            var redCube = GetComponentsInChildren<Transform>()
                          .FirstOrDefault(t => t != transform && t.CompareTag("Red"));
            if (redCube != null)
            {
                cubeTransform = redCube;
                Debug.Log("Red kocka megtalálva");
            }
            else
            {
                Debug.LogError("CubeMover: Nem található gyerekek között 'Red' tag-gel rendelkező objektum!");
                return;
            }
        }

        //cube mentése
        if (cubeTransform != null)
            initialCubePos = cubeTransform.position;

        // faceQuads[0] == self, szóval kiszűrjük
        faceQuads = cubeTransform.GetComponentsInChildren<Transform>()
                             .Where(t => t != cubeTransform && t.GetComponent<Renderer>() != null)
                             .ToArray();
    }

    /// <summary>
    /// Kilistázza a sorban maradt move-okat és kitörli,
    /// a marker-eket is, és a kockát visszaállítja a kezdeti helyére.
    /// </summary>
    public void ResetCube()
    {
        // 1) töröljük a még ki nem játszott mozdulatokat
        moveQueue.Clear();
        // 2) töröljük a lépésmarker-eket
        ClearPreviousMarkers();
        // 3) állítsuk vissza a piros kockát a start pozícióra
        if (cubeTransform != null)
        {
            cubeTransform.position = initialCubePos;
            // ha szeretnéd, itt visszaállíthatod a faces színeit is
        }
    }

    public void EnqueueMoves(List<(Vector3 pos, int redFaceIndex)> moves)
    {

        // 1) előző marker-ek törlése
        ClearPreviousMarkers();

        // 2) queue feltöltése
        moveQueue.Clear();

        foreach (var move in moves)
            moveQueue.Enqueue(move);

        if (!isMoving)
            StartCoroutine(ProcessMoves());
    }

    public void ClearPreviousMarkers()
    {
        foreach (var m in spawnedMarkers)
            if (m != null) Destroy(m);
        spawnedMarkers.Clear();
    }

    private IEnumerator ProcessMoves()
    {
        Initialize();
        isMoving = true;
        int stepCount = 0;

        while (moveQueue.Count > 0)
        {

            if (cubeTransform == null)
            {
                Debug.LogWarning("CubeMover: A kocka (cubeTransform) már nem él – megszakítjuk a mozgást.");
                isMoving = false;
                yield break;
            }

            float delay = 1f / Mathf.Max(movesPerSecond, 0.01f); // ne legyen osztás 0-val
            var (targetPos, redFace) = moveQueue.Dequeue();

            SetRedFace(redFace);
            //DebugRedFaceDirection(redFace);
            cubeTransform.position = targetPos;
            stepCount++;
            stepUpdatedEvent?.Raise(stepCount);




            if (stepMarkerPrefab != null)
            {
                Vector3 markerPos = targetPos + Vector3.up * markerHeightOffset;
                var marker = Instantiate(stepMarkerPrefab, markerPos, Quaternion.identity);

                // ha akarjuk, felülírjuk a prefab fade-ét az editorban beállítottal
                if (overrideMarkerFade)
                {
                    var sm = marker.GetComponent<StepMarker>();
                    if (sm != null)
                    {
                        sm.enabled = true;
                        sm.GetType()
                          .GetField("enableFade", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                          .SetValue(sm, defaultEnableFade);
                        sm.GetType()
                          .GetField("fadeDuration", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                          .SetValue(sm, defaultFadeDuration);
                    }
                }

                // szöveg beállítása és billboarding
                var tmp = marker.GetComponentInChildren<TMPro.TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = stepCount.ToString();
                    tmp.transform.rotation = Camera.main.transform.rotation;
                }
                else if (marker.TryGetComponent(out TextMesh tm))
                {
                    tm.text = stepCount.ToString();
                    marker.transform.rotation = Camera.main.transform.rotation;
                }

                spawnedMarkers.Add(marker);
            }










            yield return new WaitForSeconds(delay);
        }




        isMoving = false;
    }

    private void SetRedFace(int redFaceIndex)
    {
        for (int i = 0; i < faceQuads.Length; i++)
        {
            var renderer = faceQuads[i].GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = (i == redFaceIndex) ? redMaterial : blueMaterial;
        }
    }

    private void DebugRedFaceDirection(int redFaceIndex)
    {
        if (faceQuads == null || redFaceIndex < 0 || redFaceIndex >= faceQuads.Length)
        {
            Debug.LogWarning("DebugRedFaceDirection: érvénytelen index vagy nincs faceQuads.");
            return;
        }

        Transform redFace = faceQuads[redFaceIndex];
        Vector3 worldNormal = redFace.forward; // vagy lehet redFace.up, ha úgy áll a quad

        // Melyik világirányhoz van legközelebb?
        var dirs = new Dictionary<string, Vector3>
    {
        { "+Y (Up)", Vector3.up },
        { "-Y (Down)", Vector3.down },
        { "+Z (Forward)", Vector3.forward },
        { "-Z (Back)", Vector3.back },
        { "+X (Right)", Vector3.right },
        { "-X (Left)", Vector3.left },
    };

        string closest = "";
        float maxDot = -1f;
        foreach (var kvp in dirs)
        {
            float dot = Vector3.Dot(worldNormal.normalized, kvp.Value);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = kvp.Key;
            }
        }

        Debug.Log($"[DEBUG] Piros oldal (face index: {redFaceIndex}) iránya: {closest} (dot: {maxDot:F2})");
    }

}
