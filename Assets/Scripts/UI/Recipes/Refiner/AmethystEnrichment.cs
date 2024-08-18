using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmethystEnrichment : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AmethystFragment), 5, 0), 
            new Recipe.ItemBatch(typeof(AzuriteFragment), 2, 0)
        };
		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AmethystShard), 2, 3), 
            new Recipe.ItemBatch(typeof(AmethystShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
