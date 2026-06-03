using System.Collections;
using UnityEngine;

public class ClickTarget : MonoBehaviour
{
    [SerializeField] private float growthSpeed = 2f;
    [SerializeField] private float fadeSpeed = 5f;

    private Transform gfx;
    private SpriteRenderer sr;

    private void Awake()
    {
        gfx = transform.Find("GFX");
        sr = gfx.GetComponent<SpriteRenderer>();

        StartCoroutine(AnimateTarget());
    }

    private IEnumerator AnimateTarget()
    {
        float baseScale = gfx.localScale.x;
        gfx.localScale = Vector3.zero;

        float currentScale = 0f;

        while (currentScale < baseScale)
        {
            currentScale += Time.deltaTime * growthSpeed;
            gfx.localScale = Vector3.one * currentScale;
            yield return null;
        }

        gfx.localScale = Vector3.one * baseScale;

        Color myColor = sr.color;

        while (myColor.a > 0f)
        {
            myColor.a -= Time.deltaTime * fadeSpeed;
            sr.color = myColor;
            yield return null;
        }

        Destroy(gameObject);
    }
}