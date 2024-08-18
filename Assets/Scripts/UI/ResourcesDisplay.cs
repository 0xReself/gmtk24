using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesDisplay : MonoBehaviour {
    
    [SerializeField]
    private List<GameObject> resources;

    [SerializeField]
    private List<GameObject> resourcesText;

    [SerializeField]
    private List<Resource> resourceType;

    [SerializeField]
    private ResourceManager resourceManager;

    void Start() {
    }

    public bool BoxNeeded() {
        for (int i = 0; i < resources.Count; i++) {
            if (resourceManager.GetResource(resourceType[i]) <= 0) {
                continue;
            }
            return true;
        }
        return false;
    }

    void Update() {
        if(BoxNeeded()) {
            this.gameObject.GetComponent<Image>().color = new Color(0.149f, 0.149f, 0.149f, 1f);
        } else {
            this.gameObject.GetComponent<Image>().color = new Color(0.149f, 0.149f, 0.149f, 0f);
        }

        for (int i = 0; i < resources.Count; i++) {
            if (resourceManager.GetResource(resourceType[i]) <= 0) {
                resources[i].SetActive(false);
                continue;
            }

            resources[i].SetActive(true);
            resourcesText[i].GetComponent<TextMeshProUGUI>().text = ""+resourceManager.GetResource(resourceType[i]);
        }
    }
}
