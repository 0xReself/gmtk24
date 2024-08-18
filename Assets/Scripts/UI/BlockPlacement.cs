using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPlacement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    private PlacementController placementController;
    
    void Awake() {
        placementController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlacementController>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        placementController.disabled = true;
        placementController.CloseHover();
        placementController.CloseClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        placementController.disabled = false;
    }
}
