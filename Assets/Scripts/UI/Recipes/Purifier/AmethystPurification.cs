using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmethystPurification : ReceiptCard {
    
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AmethystFragment), 2, 0), 
            new Recipe.ItemBatch(typeof(AmethystShard), 5, 0), 
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AmethystEssence), 1, 3), 
            new Recipe.ItemBatch(typeof(AmethystEssence), 0, 4) 
        };

		recipe = new Recipe(inputItems, outputItems, 100);
    }
    
    void Update() {
        
    }
}
