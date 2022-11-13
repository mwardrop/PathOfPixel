using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    public Button CloseButton;

    protected virtual void Start()
    {
        CloseButton.onClick.AddListener(CloseClicked);
    }

    protected virtual void Update()
    {
        
    }

    private void CloseClicked()
    {
        gameObject.SetActive(false);

    }

}
