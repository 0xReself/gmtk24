using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    protected GameObject shop;

    [SerializeField]
    protected ShopManager shopManager;

    [SerializeField]
    protected bool activated = false;

    [SerializeField]
    protected List<Sprite> sprites;

    [SerializeField]
    protected Image icon;

    [SerializeField]
    protected AudioSource hover;

    [SerializeField]
    protected AudioSource clicked;

    public virtual void SetClosed() {
        GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f);
        icon.sprite = sprites[0];
        activated = false;
    }

    public virtual void SetOpen() {
        GetComponent<Image>().color = new Color(0.961f, 0.961f, 0.961f);
        icon.sprite = sprites[1];
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        clicked.Play();
        if (activated == true) {
            shopManager.CloseOpenUI();
            activated = false;
            return;
        }

        shopManager.CloseOpenUI();
        SetOpen();
        shop.SetActive(true);
        activated = true;
    }

    void Start() {
        
    }

    void Update() {
        
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hover.Play();
        if (activated == false) {
            GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (activated == false) {
            SetClosed();
        }
    }
}
