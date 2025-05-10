using System.Collections;
using TMPro; // ha TextMeshPro-t használsz
using UnityEngine;

public class StepMarker : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private bool enableFade = true;
    [SerializeField] private float fadeDuration = 1f;

    private TMP_Text tmp;      // TextMeshPro-kompontens
    private TextMesh tm;      // sima TextMesh-komponens
    private Color initialColor;

    void Awake()
    {
        // megkeressük a szöveget
        tmp = GetComponentInChildren<TMP_Text>();
        if (tmp != null) initialColor = tmp.color;
        else if (TryGetComponent(out tm)) initialColor = tm.color;
        else
        {
            Debug.LogWarning("StepMarker: nincs TextMeshPro vagy TextMesh komponens!");
            enableFade = false;
        }
    }

    void OnEnable()
    {
        if (enableFade)
            StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(initialColor.a, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void SetAlpha(float a)
    {
        if (tmp != null)
        {
            var c = tmp.color;
            c.a = a;
            tmp.color = c;
        }
        else if (tm != null)
        {
            var c = tm.color;
            c.a = a;
            tm.color = c;
        }
    }
}
