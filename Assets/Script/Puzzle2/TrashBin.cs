using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBin : MonoBehaviour, IDropHandler
{
    [Header("Bin Config")] 
    [SerializeField] private string acceptedCategory;

    public void OnDrop(PointerEventData eventData)
    {
        DragDropPuzzle droppedItem = eventData.pointerDrag.GetComponent<DragDropPuzzle>();

        if (droppedItem != null)
        {
            //ngecek kategori sampahny
            if (droppedItem.trashCategory == acceptedCategory)
            {
                droppedItem.transform.SetParent(this.transform);
                droppedItem.gameObject.SetActive(false);

                PuzzletwoManager.instance.ItemSorted();
            }
            else
            {
                droppedItem.ResetPosition();
            }
        }
    }
}
