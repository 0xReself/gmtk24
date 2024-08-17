using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SidePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    [SerializeField]
    private PlacementController placementController;
    
    public void OnPointerEnter(PointerEventData eventData) {
        placementController.disabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        placementController.disabled = false;
    }
}
