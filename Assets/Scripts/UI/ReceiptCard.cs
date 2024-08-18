using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiptCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        
    [SerializeField]
    private Recipe recipe;

    [SerializeField]
    private GameObject recipeFull;

    private bool selected = false;

    public void OnPointerClick(PointerEventData eventData) {
        recipeFull.SetActive(true);
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 1.0f);
        selected = true;
        //this.GetComponent<Purifier>().setRecipe(recipe);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(selected == true) {
            return;
        }
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(selected == true) {
            return;
        }
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 0.0f);
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
