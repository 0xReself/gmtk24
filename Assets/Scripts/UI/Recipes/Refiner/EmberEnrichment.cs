using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberEnrichment : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(CrimsonFragment), 10, 0), 
            new Recipe.ItemBatch(typeof(AzuriteShard), 5, 0), 
            new Recipe.ItemBatch(typeof(CrimsonShard), 7, 0), 
            new Recipe.ItemBatch(typeof(AmethystEssence), 10, 0), 
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(EmberShard), 10, 3), 
            new Recipe.ItemBatch(typeof(EmberShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);   
    }

    void Update() {
        
    }
}
