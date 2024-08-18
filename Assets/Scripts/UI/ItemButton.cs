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

    [SerializeField]
    private List<Resource> resourcesToBuy;

    [SerializeField]
    private List<int> resourceAmount;

    private ResourceManager resourceManager;
    
    void Awake() {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        click.Play();
        bool canBuy = true;
        for (int i = 0; i < resourceAmount.Count; i++) {
            if(!resourceManager.CanRemove(resourcesToBuy[i], resourceAmount[i])) {
                canBuy = false;
            }
        }

        if(canBuy) {
            Buy();
        }
    }

    public void Buy() {
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
