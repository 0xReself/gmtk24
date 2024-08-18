using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShopManager : MonoBehaviour {
    
    [SerializeField]
    private List<GameObject> stores;

    [SerializeField]
    private List<GameObject> buttons;

    public void CloseOpenUI() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UIToClose")) {
            obj.SetActive(false);
        }

        foreach (GameObject store in stores) {
            store.SetActive(false);
        }

        foreach (GameObject button in buttons) {
            CategoryButton category = button.GetComponent<CategoryButton>();
            category.SetClosed();
        }
    }
}
