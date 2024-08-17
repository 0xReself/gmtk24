using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableButton : MonoBehaviour {
    [SerializeField]
    private string placeableName;

    [SerializeField]
    private GameObject placeablePrefab;

    [SerializeField]
    private PlacementController placementController;

    void Start() {
        this.GetComponentInChildren<TextMeshProUGUI>().text = placeableName;
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() {
        placementController.ChangeSelectedPlaceable(placeablePrefab, PlacingMode.Building);
    }
}
