using UnityEngine;
using TMPro;

public class FeedbackText : MonoBehaviour
{
    [SerializeField] private float lifespan = 2f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float fadeSpeed = 0.5f;

    private TextMeshPro myText;

    private void Awake()
    {
        myText = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        transform.localPosition += Vector3.forward * Time.deltaTime * moveSpeed * lifespan;

        SetFadingText();
    }

    private void SetFadingText()
    {
        Color c = myText.color;
        c.a -= Time.deltaTime * fadeSpeed * lifespan;
        myText.color = c;
    }

    public void ChangeText(float value)
    {
        myText.text = Mathf.Round(value).ToString();

        if (value > 0)
        {
            myText.color = Color.green;
        }
        else
        {
            myText.color = Color.red;
        }
    }
}