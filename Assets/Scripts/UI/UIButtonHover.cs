using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.15f, 1.15f, 1f);
    public float speed = 10f;

    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(hoverScale, hoverColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(normalScale, normalColor));
    }

    System.Collections.IEnumerator ScaleTo(Vector3 targetScale, Color targetColor)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
            img.color = Color.Lerp(img.color, targetColor, Time.deltaTime * speed);
            yield return null;
        }
    }
}