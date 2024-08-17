using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour {
    
    [SerializeField]
    public Vector2Int size = Vector2Int.zero;

    [SerializeField]
    public int rotation = 0;

    [SerializeField]
    private GameObject baseLayer;

    [SerializeField]
    private GameObject topLayer;

    public void SetColor(Color color) {
        baseLayer.GetComponent<SpriteRenderer>().color = color;
        
        if (topLayer != null) {
            topLayer.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetSortingLayer(int layer) {
        baseLayer.GetComponent<SpriteRenderer>().sortingOrder = layer;

        if (topLayer != null) {
            topLayer.GetComponent<SpriteRenderer>().sortingOrder = layer;
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
            //Right
            case 1:
                return new Vector2Int(startPosition.x + sizedFixed.y, startPosition.y + sizedFixed.x);
              
            //Top
            case 2:
                return new Vector2Int(startPosition.x - sizedFixed.x, startPosition.y + sizedFixed.y);

            //Left
            case 3:
                return new Vector2Int(startPosition.x - sizedFixed.y, startPosition.y - sizedFixed.x);

            //Default rotation splitter goes down e.g.
            default:
                return new Vector2Int(startPosition.x + sizedFixed.x, startPosition.y - sizedFixed.y);
        }
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
