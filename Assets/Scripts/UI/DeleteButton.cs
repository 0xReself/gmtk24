using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteButton : CategoryButton {
    
    [SerializeField]
    private PlacementController placementController;

    void Start() {
        
    }

    public override void SetOpen() {
        GetComponent<Image>().color = new Color(1f, 0f, 0f);
    }

    public override void OnPointerClick(PointerEventData eventData) {
        shopManager.CloseOpenUI();
        SetOpen();
        placementController.ChangeSelectedPlaceable(null, PlacingMode.Deleting);
    }
}
