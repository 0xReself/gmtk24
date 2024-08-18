using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiptCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        
    [SerializeField]
    protected Recipe recipe;

    [SerializeField]
    protected List<Sprite> sprites;

    protected bool selected = false;

    [SerializeField]
    protected RecipeManager recipeManager;

    [SerializeField]
    protected PlacementController placementController;

    public void ResetSelected() {
        this.GetComponent<Image>().sprite = sprites[0];
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 0.0f);
        selected = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        recipeManager.ResetSelected();
        this.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1.0f);
        this.GetComponent<Image>().sprite = sprites[1];
        selected = true;
        recipeManager.gameObject.SetActive(false);
        placementController.disabled = false;
        placementController.ChangeSelectedPlaceable(null, PlacingMode.Idle);
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

    void Awake() {
        this.placementController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlacementController>();
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
