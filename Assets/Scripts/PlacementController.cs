using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementController : MonoBehaviour {
    
    [SerializeField]
    private GameObject selectedPlaceablePrefab;

    [SerializeField]
    private CameraController cameraController;
    
    [SerializeField]
    private MapManager mapManager;

    private GameObject placingObject;

    private int rotationStep = 0;

    void Start() {
        cameraController = this.GetComponent<CameraController>();
        ResetPlacingObject();
    }

    private void ResetPlacingObject() {
        placingObject = Instantiate(selectedPlaceablePrefab, cameraController.GetCenterGridPosition(), Quaternion.identity);
    }

    private Vector2Int ToVector2Int(Vector3 vector) {
        return new Vector2Int(
                    (int)vector.x,
                    (int)vector.y
                );
    }

    private bool CanPlace(Vector2Int start, Vector2Int end) {
        for(int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++) {
            for(int y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y++) {
                if(mapManager.Get(new Vector2Int(x, y)) != null) {
                    return false;
                }
            }
        }
        return true; 
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
        Placeable placeable = placingObject.GetComponent<Placeable>();
        placeable.spriteRenderer.sortingOrder = -(int)cameraController.GetGridPosition().y;

        Vector2Int size = placingObject.GetComponent<Placeable>().size - Vector2Int.one;
        Vector2Int positionStart = ToVector2Int(cameraController.GetGridPosition());
        Vector2Int positionEnd = ToVector2Int(cameraController.GetGridPosition()) + size;

        bool canPlace = CanPlace(positionStart, positionEnd);

        if (!canPlace) {
            placeable.spriteRenderer.color = new Color(1f, 0.25f, 0.25f);
        } else {
            placeable.spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && canPlace) {
            Place(placeable, positionStart, positionEnd);
        }
    }

    private Vector2Int Rotate90Degrees(Vector2Int vector) {
        return new Vector2Int(-vector.y, vector.x);
    }

    private void HandleRotation() {
        if (Input.GetKeyDown(KeyCode.R)) {
            rotationStep += 1;
            if(rotationStep >= 4) {
                rotationStep = 0;
            }
            placingObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationStep * 90));
            
            //Completly Wrong
            Placeable placeable = placingObject.GetComponent<Placeable>();
            placeable.size = Rotate90Degrees(placeable.size);
        }
    }

    void Update() {
        HandlePlaceable();
        HandleRotation();
    }
}
