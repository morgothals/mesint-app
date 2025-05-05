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

    private Queue<(Vector3 targetPosition, int redFaceIndex)> moveQueue = new();
    private bool isMoving = false;
    private Transform[] faceQuads; // a 6 quad a kocka alá


    void Start()
    {
        Initialize();
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



        // faceQuads[0] == self, szóval kiszűrjük
        faceQuads = cubeTransform.GetComponentsInChildren<Transform>()
                             .Where(t => t != cubeTransform && t.GetComponent<Renderer>() != null)
                             .ToArray();
    }

    public void EnqueueMoves(List<(Vector3 pos, int redFaceIndex)> moves)
    {
        foreach (var move in moves)
            moveQueue.Enqueue(move);

        if (!isMoving)
            StartCoroutine(ProcessMoves());
    }

    private IEnumerator ProcessMoves()
    {
        Initialize();
        isMoving = true;

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
            DebugRedFaceDirection(redFace);
            cubeTransform.position = targetPos;

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
