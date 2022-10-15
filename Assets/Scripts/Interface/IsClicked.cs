using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IsClicked : MonoBehaviour
{
    private MovementIndicator movementIndicator;
    public GameObject movementIndicatorObject;
    // Start is called before the first frame update
    void Start()
    {
        movementIndicator = movementIndicatorObject.GetComponent<MovementIndicator>();
        foreach (Transform child in transform)
        {
            IsClicked childIsClicked = transform.AddComponent<IsClicked>();
            childIsClicked.movementIndicatorObject = movementIndicatorObject;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        movementIndicator.interfaceClicked = true;
    }

    private void OnMouseUp()
    {
        movementIndicator.interfaceClicked = false;
    }
}
