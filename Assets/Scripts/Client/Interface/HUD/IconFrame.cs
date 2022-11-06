using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconFrame : MonoBehaviour, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData) // 3
    {
        gameObject.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetButtons", 0.1f);
    }

    private void ResetButtons()
    {
        gameObject.transform.Find("Clicked").gameObject.SetActive(false);
    }
}
