using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Placeable : MonoBehaviour {
    
    // the current position of the placeable inside of the map of mapmanager
    public Vector2Int startPosition = Vector2Int.zero;

	// default 1:1 is x:y default square. can be of any different sizes
	public Vector2Int size = Vector2Int.zero;

    // Rotation from 0 to 4 (inclusive) multiplied by 90 to get degrees: 
    // Default rotation 0 = left to right (input to output),  1 = top to bot, 2 = right to left, 3 = down to top
    //
    // anchor point is top left 
    public int rotation = 0;

    [SerializeField]
    private GameObject baseLayer;

    [SerializeField]
    protected GameObject topLayer;

    [SerializeField]
    protected float pushUp = 4;
    
    [SerializeField]
    protected float spawnTime = 0.2f;

    protected float time_elapsed = 0;

    protected bool animate = false;

    // preview 
    protected bool isPreview = true;

    public void SetColor(Color color) {
        baseLayer.GetComponent<SpriteRenderer>().color = color;
        
        if (topLayer != null) {
            topLayer.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetSortingLayer(int layer) {
        baseLayer.GetComponent<SpriteRenderer>().sortingOrder = layer;

        if (topLayer != null) {
            topLayer.GetComponent<SpriteRenderer>().sortingOrder = layer + 1;
        }
    }

    private void SetOpacity(float opacity) {
        baseLayer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
        
        if (topLayer != null) {
            topLayer.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, opacity);
        }
    }

    public void Rotate() {
        if (topLayer == null) {
            return;
        }

        rotation += 1;
        if (rotation >= 4) {
            rotation = 0;
        }

        topLayer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -rotation * 90));
    }

    public void Rotate(int rotations) {
        if (topLayer == null) {
            return;
        }

        rotation += rotations;
        if (rotation >= 4) {
            rotation = 0;
        }

        topLayer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -rotation * 90));
    }

    public Vector2Int GetEndPosition(Vector2Int startPosition) {
        Vector2Int sizedFixed = size - Vector2Int.one;

        switch (rotation) {
            case 1:
                return new Vector2Int(startPosition.x + sizedFixed.y, startPosition.y + sizedFixed.x);
                
            case 2:
                return new Vector2Int(startPosition.x - sizedFixed.x, startPosition.y + sizedFixed.y);

            case 3:
                return new Vector2Int(startPosition.x - sizedFixed.y, startPosition.y - sizedFixed.x);

            default:
                return new Vector2Int(startPosition.x + sizedFixed.x, startPosition.y - sizedFixed.y);
        }
    }

    void Start() {}

    void Update() {
        if (animate) {
            HandleSpawnAnimation();
        }
    }

    public void HandleSpawnAnimation() {
        float spawnStartY = startPosition.y + pushUp;
        float delta = 1 - Mathf.Sqrt(1 - time_elapsed / spawnTime);
        float newY = Mathf.Lerp(spawnStartY, startPosition.y + 0.5f, delta);
        SetOpacity(delta);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        time_elapsed += Time.deltaTime;
        if (time_elapsed > spawnTime) {
            this.GetComponent<AudioSource>().Play();
            SetOpacity(1);
            transform.position = new Vector3(startPosition.x + 0.5f, startPosition.y + 0.5f);
            animate = false;
            return;
        }
    }

    public void StartSpawn() {
        animate = true;
        time_elapsed = 0;
        isPreview = false;
    }

    // currently returns only one dimension of the size vector, because only quadratic shapes are supported!
    public int GetSize() {
        return size.x;
    }

    // how many times they are rotated (0 to 3)
    public int GetRotation() {
        return rotation;
    }

    // if this is finished animating and is not a preview
    public bool isAlive()
    {
        return animate == false && isPreview == false;
    }
}
