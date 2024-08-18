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

    void Awake() {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }
    
    public override void OnClickDown() {
        resourceManager.AddResource(Resource.Azurite, 1);
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
    }

    public override void OnHoverEnd() {
        hover = false;
        baseLayer.GetComponent<SpriteRenderer>().sprite = sprites[0];
    }

    void Start() {
        infoText = GameObject.FindGameObjectWithTag("TutorialText");
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            OnClickDown();
        }

        if(Input.GetKeyUp(KeyCode.Space)) {
            OnClickUp();
        }
    }

}
