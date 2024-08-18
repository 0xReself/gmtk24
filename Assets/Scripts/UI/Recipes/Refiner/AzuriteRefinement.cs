using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzuriteRefinement : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AzuriteFragment), 10, 0)};
		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(AzuriteShard), 1, 3), 
            new Recipe.ItemBatch(typeof(AzuriteShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
