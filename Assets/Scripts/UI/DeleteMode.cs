using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteMode : MonoBehaviour {
    
    [SerializeField]
    private PlacementController placementController;

    void Start() {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick() {
        placementController.ChangeSelectedPlaceable(null, PlacingMode.Deleting);
    }
}
