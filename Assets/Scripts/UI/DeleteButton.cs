using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeleteButton : CategoryButton {
    
    [SerializeField]
    private PlacementController placementController;

    void Start() {
        
    }

    public override void SetOpen() {
        GetComponent<Image>().color = new Color(0.937f, 0.267f, 0.267f);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.D)) {
            Click();
        }
    }

    public override void OnPointerClick(PointerEventData eventData) {
        Click();
    }

    public void Click() {
        clicked.Play();
        if (activated == true) {
            shopManager.CloseOpenUI();
            placementController.ChangeSelectedPlaceable(null, PlacingMode.Idle);
            activated = false;
            return;
        }

        shopManager.CloseOpenUI();
        SetOpen();
        placementController.ChangeSelectedPlaceable(null, PlacingMode.Deleting);
        activated = true;
    }
}
