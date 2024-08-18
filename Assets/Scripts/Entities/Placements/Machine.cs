using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machines : Placeable {
    [SerializeField]
    private GameObject receipeUI;

    private ShopManager uiManager;

    void Awake() {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<ShopManager>();
        //placementManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlacementController>();
    }

    void Start() {
    }

    void Update() {
        if (animate) {
            HandleSpawnAnimation();
        }
    }

    public override void OnClickDown() {
        uiManager.CloseOpenUI();
        //placementManager.ChangeSelectedPlaceable(null, PlacingMode.Idle);
        receipeUI.SetActive(true);
    }
}
