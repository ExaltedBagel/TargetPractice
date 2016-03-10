using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.placeholderParent = this.transform;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null && d.placeholderParent == this.transform)
        {
            d.placeholderParent = d.parentToReturnTo;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        if (d != null)
        {
            d.parentToReturnTo = this.transform;
        }
        if (this.gameObject.name == "DropArea")
        {
            Card card = d.gameObject.GetComponent<Card>();
            if (UnitHandler.currentHero.canPlayCard(card))
            {
                HarvestedCrystal.setUsedAlpha(card.manaCost);
                d.GetComponent<Card>().playCard();
            }
            else
            {
                Debug.Log(gameObject.transform.GetChild(0).name);
                Destroy(gameObject.transform.FindChild("placeholder").gameObject);
                if(card != null)
                {
                    Debug.Log("Card not null " + card.name + " ID " + card.gameObject.GetInstanceID());
                    card.gameObject.GetComponent<Draggable>().returnToPosition();
                }
                    
            }
        }          

    }
}
