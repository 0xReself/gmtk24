using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    private GameObject objectPrefab;
    
    [SerializeField]
    private PlacementController placementController;

    [SerializeField]
    private ShopManager shopManager;

    [SerializeField]
    protected AudioSource hover;

    [SerializeField]
    protected AudioSource click;
    
    public void OnPointerClick(PointerEventData eventData) {
        click.Play();
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 0.0f);
        shopManager.CloseOpenUI();
        placementController.disabled = false;
        placementController.ChangeSelectedPlaceable(objectPrefab, PlacingMode.Building);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hover.Play();
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        this.GetComponent<Image>().color = new Color(0.09f, 0.09f, 0.09f, 0.0f);
    }
}
