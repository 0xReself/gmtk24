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

    [SerializeField]
    protected AudioSource hoverSound;

    bool firstClick = true;

    [SerializeField]
    protected GameObject infoText;

    private bool hover = false;

    public override void OnClickDown() {
        if(firstClick == true) {
            infoText.SetActive(false);
            firstClick = false;
        }

        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[1];
        keyDown.Play();
    }

    public override void OnClickUp() {
        if(hover == true) {
            baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[2];
        } else {
            baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
        keyUp.Play();
    }

    public override void OnHoverStart() {
        hover = true;
        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[2];
        hoverSound.Play();
        Debug.Log("Hover Start");
    }

    public override void OnHoverEnd() {
        hover = false;
        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[0];
        Debug.Log("Hover End");
    }

    void Start() {
        infoText = GameObject.FindGameObjectWithTag("TutorialText");
    }

    // Update is called once per frame
    void Update() {
        
    }

}
