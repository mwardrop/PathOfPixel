using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipHoverBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private GameObject Background { get { return transform.parent.Find("Canvas").Find("Background").gameObject; } }
    private GameObject Label { get { return transform.parent.Find("Canvas").Find("TextLabel").gameObject; } }
    private GameObject Description { get { return transform.parent.Find("Canvas").Find("TextDescription").gameObject; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void showToolTip()
    {
        Label.SetActive(true);
        Description.SetActive(true);
        Background.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Invoke("showToolTip", 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke("showToolTip");
        Label.SetActive(false);
        Description.SetActive(false);
        Background.SetActive(false);
    }
}
