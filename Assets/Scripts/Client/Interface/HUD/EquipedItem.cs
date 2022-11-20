using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipedItem : MonoBehaviour, IDropHandler
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {

        //Icon dropIcon = eventData.pointerDrag.GetComponent<Icon>();

        //ClientManager.Instance.StateManager.Actions.SetPlayerHotbarItem(dropIcon.TypeKey, dropIcon.Type, HotbarIndex);
    }

}
