using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlacingMode {
    Idle = 0,
    Building = 1,
    Deleting = 2,
}

public class PlacementController : MonoBehaviour {
    
    [SerializeField]
    public GameObject selectedPlaceablePrefab = null;

    [SerializeField]
    private CameraController cameraController;
    
    [SerializeField]
    private MapManager mapManager;

    [SerializeField]
    public PlacingMode placingMode;

    private GameObject placingObject;
    private Placeable placeable;

    public bool disabled = false;

    private int currentRotation = 0;

    void Start() {
        cameraController = this.GetComponent<CameraController>();
        ResetPlacingObject();
    }

    public void ChangeSelectedPlaceable(GameObject prefab, PlacingMode placingMode) {
        this.placingMode = placingMode;
        selectedPlaceablePrefab = prefab;
        Destroy(placingObject);
        ResetPlacingObject();
    }

    private void ResetPlacingObject() {
        if (selectedPlaceablePrefab == null) {
            return;
        }

        placingObject = Instantiate(selectedPlaceablePrefab, cameraController.GetCenterGridPosition(), Quaternion.identity);
        placeable = placingObject.GetComponent<Placeable>();

        if (placeable == null) {
            Debug.LogError("Trying to place object with no Placeable script!");
        }

        placeable.Rotate(currentRotation);
    }

    private Vector2Int ToVector2Int(Vector3 vector) {
        return new Vector2Int(
                    (int)vector.x,
                    (int)vector.y
                );
    }

    private void Place(Placeable placeable, Vector2Int start, Vector2Int end) {
        placeable.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        if (mapManager.Get(start) != null) {
            return;
        }

        mapManager.Set(placingObject, start, end);

        ResetPlacingObject();
    }
    private void HandlePlaceable() {
        placingObject.transform.position = cameraController.GetCenterGridPosition();
        placeable.spriteRenderer.sortingOrder = -(int)cameraController.GetGridPosition().y;

        Vector2Int positionStart = ToVector2Int(cameraController.GetGridPosition());
        Vector2Int positionEnd = placeable.GetEndPosition(positionStart);

        bool canPlace = mapManager.CanPlace(positionStart, positionEnd);

        if (!canPlace) {
            placeable.spriteRenderer.color = new Color(1f, 0.25f, 0.25f);
        } else {
            placeable.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        if (Input.GetKey(KeyCode.Mouse0) && canPlace && !disabled) {
            Place(placeable, positionStart, positionEnd);
        }
    }

    private void HandleDelete() {
        Vector2Int position = ToVector2Int(cameraController.GetGridPosition());
        GameObject placedObject = mapManager.Get(position);
        
        if (placedObject != null) {
            Placeable _placeable = placedObject.GetComponent<Placeable>();

            if(Input.GetKey(KeyCode.Mouse0) && ! disabled) {
                mapManager.Remove(position, _placeable.GetEndPosition(position));
                Destroy(placedObject);
            }
        }
    }

    private void HandleRotation() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Placeable placeable = placingObject.GetComponent<Placeable>();
            placeable.Rotate();
            currentRotation = placeable.rotation;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ChangeSelectedPlaceable(null, PlacingMode.Idle);
        }

        if (selectedPlaceablePrefab != null && placingMode == PlacingMode.Building) {
            HandlePlaceable();
            HandleRotation();
        }        
        
        if(placingMode == PlacingMode.Deleting) {
            HandleDelete();
        }
    }
}
