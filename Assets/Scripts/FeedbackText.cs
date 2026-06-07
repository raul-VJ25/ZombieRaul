using UnityEngine;
using TMPro;

public class FeedbackText : MonoBehaviour
{
    [SerializeField] private float lifespan = 2f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float fadeSpeed = 0.5f;

    [SerializeField] private TMP_Text myText;

    void Awake()
    {
        if (myText == null)
        {
            myText = GetComponentInChildren<TMP_Text>();
        }

        if (myText == null)
        {
            Debug.LogError("¡ERROR en " + gameObject.name + "! No se encuentra ningún componente de texto (TMP).");
        }
        else
        {
            Destroy(gameObject, lifespan);
        }
    }

    void Update()
    {
        if (myText != null)
        {
            transform.localPosition += Vector3.forward * Time.deltaTime * moveSpeed * lifespan;
            SetFadingText();
        }
    }

    void SetFadingText()
    {
        Color c = myText.color;
        c.a -= Time.deltaTime * fadeSpeed * lifespan;
        myText.color = c;
    }

    public void ChangeText(float value)
    {
        if (myText != null)
        {
            myText.text = Mathf.Round(value).ToString();

            myText.color = value > 0 ? Color.green : Color.red;
        }
    }
}