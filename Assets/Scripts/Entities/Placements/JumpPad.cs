using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : Placeable
{
    [SerializeField]
    protected List<Sprite> sprites;

    void Start() {
        
    }

    
    void Update() {
        if(isPreview) {
            topLayer.GetComponent<SpriteRenderer>().sprite = sprites[1];
        } else {
            topLayer.GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
