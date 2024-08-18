using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Core : Placeable {
    
    [SerializeField]
    protected List<Sprite> sprites;

    [SerializeField]
    protected AudioSource keyUp;

    [SerializeField]
    protected AudioSource keyDown;

    bool firstClick = true;

    [SerializeField]
    protected GameObject infoText;

    public override void OnClickDown() {
        if(firstClick == true) {
            infoText.SetActive(false);
            firstClick = false;
        }

        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[1];
        keyDown.Play();
    }

    public override void OnClickUp() {
        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[0];
        keyUp.Play();
    }

    void Start() {
        infoText = GameObject.FindGameObjectWithTag("TutorialText");
    }

    // Update is called once per frame
    void Update() {
        
    }

}
