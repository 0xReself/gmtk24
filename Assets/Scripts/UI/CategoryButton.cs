using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour, IPointerClickHandler {
    [SerializeField]
    protected GameObject shop;

    [SerializeField]
    protected ShopManager shopManager;

    [SerializeField]
    protected bool activated = false;

    public virtual void SetClosed() {
        GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f);
        activated = false;
    }

    public virtual void SetOpen() {
        GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
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
}
