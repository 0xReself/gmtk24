using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    private GameObject objectPrefab;
    
    [SerializeField]
    private PlacementController placementController;

    [SerializeField]
    private ShopManager shopManager;
    
    public void OnPointerClick(PointerEventData eventData) {
        shopManager.CloseOpenUI();
        placementController.disabled = false;
        placementController.ChangeSelectedPlaceable(objectPrefab, PlacingMode.Building);
    }

}
