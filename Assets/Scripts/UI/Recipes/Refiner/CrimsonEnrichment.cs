using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonEnrichment : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(CrimsonFragment), 10, 0), 
            new Recipe.ItemBatch(typeof(AmethystFragment), 2, 0), 
            new Recipe.ItemBatch(typeof(AzuriteShard), 2, 0), 
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(CrimsonShard), 5, 3), 
            new Recipe.ItemBatch(typeof(CrimsonShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() { 

    }
}
