using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour {

    [SerializeField]
    private List<GameObject> recipes;

    public void ResetSelected() {
        foreach (GameObject obj in recipes) {
            ReceiptCard card = obj.GetComponent<ReceiptCard>();
            card.ResetSelected();
        }
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
