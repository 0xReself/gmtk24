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
    private ShopManager shopManager;
    
    [SerializeField]
    private MapManager mapManager;

    [SerializeField]
    public PlacingMode placingMode;

    private GameObject previewObject;

    public bool disabled = false;

    private int currentRotation = 0;

    [SerializeField]
    private AudioSource deleteSound;

    [SerializeField]
    private GameObject lastGameObject;

    [SerializeField]
    private GameObject lastGameObjectClicked;

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
        _placeable.SetSortingLayer(-2 * (int)cameraController.GetGridPosition().y);
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
            Placeable placeable = placedObject.GetComponent<Placeable>();

            if(placeable.startPosition == new Vector2Int(-1, 1)) {
                return;
            }

            if(Input.GetKey(KeyCode.Mouse0) && ! disabled) {
                deleteSound.Play();
                mapManager.Remove(placeable.startPosition, placeable.GetEndPosition(placeable.startPosition));
                if (placedObject.GetComponent<ItemHolder>() != null) {
                    placedObject.GetComponent<ItemHolder>().onDelete();
                }
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

    private void HandleHoverAndClick() {
        //OnHover
        GameObject objectHovered = mapManager.Get(ToVector2Int(cameraController.GetGridPosition()));
        if (objectHovered != lastGameObject) {
            if(lastGameObject != null) {
                Placeable lastPlaceable = lastGameObject.GetComponent<Placeable>();
                lastPlaceable.OnHoverEnd();
            }
            
            if (objectHovered != null) {
                Placeable newPlaceable = objectHovered.GetComponent<Placeable>();
                newPlaceable.OnHoverStart();
            }

            lastGameObject = objectHovered;
        }

        //OnClick
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            GameObject objectClicked = mapManager.Get(ToVector2Int(cameraController.GetGridPosition()));
            if(objectClicked != null) {
                lastGameObjectClicked = objectClicked;
                Placeable placeable = objectClicked.GetComponent<Placeable>();
                placeable.OnClickDown();
            } else {
                lastGameObjectClicked = null;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            if(lastGameObjectClicked != null) {
                Placeable placeable = lastGameObjectClicked.GetComponent<Placeable>();
                placeable.OnClickUp();
                lastGameObjectClicked = null;
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ChangeSelectedPlaceable(null, PlacingMode.Idle);
            shopManager.CloseOpenUI();
        }

        /*if (Input.GetKeyDown(KeyCode.Mouse0) && placingMode == PlacingMode.Idle) {
            shopManager.CloseOpenUI();
        }*/

        if (selectedPlaceablePrefab != null && placingMode == PlacingMode.Building) {
            HandlePlaceable();
            HandleRotation();
        }        
        
        if(placingMode == PlacingMode.Deleting) {
            HandleDelete();
        }

        if (placingMode == PlacingMode.Idle) {
            HandleHoverAndClick();
        }
    }
}
