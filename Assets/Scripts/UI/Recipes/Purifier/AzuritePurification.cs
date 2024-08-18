using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzuritePurification : ReceiptCard {
    
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AzuriteFragment), 2, 0), 
            new Recipe.ItemBatch(typeof(AzuriteShard), 5, 0), 
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AzuriteEssence), 1, 3), 
            new Recipe.ItemBatch(typeof(AzuriteEssence), 0, 4) 
        };

		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
