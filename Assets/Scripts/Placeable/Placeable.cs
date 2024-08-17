using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour {
    
    [SerializeField]
    public Vector2Int size = Vector2Int.zero;

    [SerializeField]
    public int rotation = 0;

    [SerializeField]
    public SpriteRenderer spriteRenderer;

    public void Rotate() {
        rotation += 1;
        if (rotation >= 4) {
            rotation = 0;
        }

        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation * 90));
    }

    public void Rotate(int rotations) {
        rotation += rotations;
        if (rotation >= 4) {
            rotation = 0;
        }

        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotation * 90));
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
