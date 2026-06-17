using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropPuzzle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Trash  Category")]
    public string trashCategory;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;

    private void Awake(){
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        startPosition = rectTransform.anchoredPosition;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == GetComponentInParent<Canvas>().transform)
        {
            rectTransform.anchoredPosition = startPosition;
        }
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = startPosition;
    }
}
