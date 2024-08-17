using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
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

    private GameObject previewObject;

    public bool disabled = false;

    private int currentRotation = 0;

    void Start() {
        cameraController = this.GetComponent<CameraController>();
    }

    public void ChangeSelectedPlaceable(GameObject prefab, PlacingMode placingMode) {
        this.placingMode = placingMode;
        selectedPlaceablePrefab = prefab;
        Destroy(previewObject);
        InitiatePreview();
    }

    private void InitiatePreview() {
        if (selectedPlaceablePrefab == null) {
            return;
        }

        previewObject = Instantiate(selectedPlaceablePrefab, cameraController.GetCenterGridPosition(), Quaternion.identity);

        if (previewObject.GetComponent<Placeable>() == null) {
            Debug.LogError("Trying to place object with no Placeable script!");
        }

        previewObject.GetComponent<Placeable>().Rotate(currentRotation);
        previewObject.GetComponent<Placeable>().SetSortingLayer(-100000000);
    }

    private Vector2Int ToVector2Int(Vector3 vector) {
        return new Vector2Int(
                    (int)vector.x,
                    (int)vector.y
                );
    }

    private void Place(Placeable placeable, Vector2Int start, Vector2Int end) {
        placeable.SetColor(new Color(1f, 1f, 1f, 1f));

        GameObject placedObject = Instantiate(selectedPlaceablePrefab, cameraController.GetCenterGridPosition(), Quaternion.identity);
        Placeable _placeable = placedObject.GetComponent<Placeable>();
        _placeable.SetSortingLayer(-(int)cameraController.GetGridPosition().y);
        _placeable.startPosition = start;
        _placeable.StartSpawn();
        _placeable.Rotate(currentRotation);

        mapManager.Set(placedObject, start, end);
    }

    private void HandlePlaceable() {
        previewObject.transform.position = cameraController.GetCenterGridPosition();
        Placeable placeable = previewObject.GetComponent<Placeable>();

        Vector2Int positionStart = ToVector2Int(cameraController.GetGridPosition());
        Vector2Int positionEnd = placeable.GetEndPosition(positionStart);

        bool canPlace = mapManager.CanPlace(positionStart, positionEnd);

        if (!canPlace) {
            placeable.SetColor(new Color(1f, 0.25f, 0.25f, 0.8f));
        } else {
            placeable.SetColor(new Color(1f, 1f, 1f, 0.8f));
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
                mapManager.Remove(_placeable.startPosition, _placeable.GetEndPosition(_placeable.startPosition));
                Destroy(placedObject);
            }
        }
    }

    private void HandleRotation() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Placeable placeable = previewObject.GetComponent<Placeable>();
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
