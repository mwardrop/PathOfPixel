using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClick : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public GameObject Panel;

    public Sprite Normal;
    public Sprite Pressed;
    public Sprite Active;

    public bool IsPressed;
    public bool IsActive { 
        get { 
            if(Panel == null) { return false; }
            return Panel.activeSelf;
        } set {
            if (Panel != null) { Panel.SetActive(value); }
        } 
    }

    private UnityEngine.UI.Image _image;
    public UnityEngine.UI.Image image
    {
        get
        {
            if (!_image) { _image = GetComponent<UnityEngine.UI.Image>(); }
            return _image;
        }
        set { _image = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPressed)
        {
            image.sprite = Pressed;
            return;
        }

        if(IsActive)
        {
            image.sprite = Active;
            return;
        }

        image.sprite = Normal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsActive = !IsActive;
        
    }

}
