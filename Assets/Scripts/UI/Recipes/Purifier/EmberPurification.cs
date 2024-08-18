using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberPurification : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(EmberFragment), 5, 0), 
            new Recipe.ItemBatch(typeof(EmberShard), 15, 0),
            new Recipe.ItemBatch(typeof(CrimsonFragment), 3, 0),
            new Recipe.ItemBatch(typeof(CrimsonEssence), 3, 0),
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(EmberEssence), 10, 3), 
            new Recipe.ItemBatch(typeof(EmberEssence), 0, 4) 
        };

		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
