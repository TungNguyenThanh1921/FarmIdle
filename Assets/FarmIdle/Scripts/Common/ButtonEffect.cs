using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float clickScale = 1.1f;  // Scale khi nhấn xuống
    public float duration = 0.15f;   // Thời gian animation

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(originalScale * clickScale, duration).SetEase(Ease.InOutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
    }
}
