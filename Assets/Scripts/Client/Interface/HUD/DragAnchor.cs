using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAnchor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool dragging;
    private Vector2 offset;

    public void Update()
    {
        if (dragging)
        {
            gameObject.transform.parent.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        offset = eventData.position - new Vector2(gameObject.transform.parent.transform.position.x, gameObject.transform.parent.transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

}
