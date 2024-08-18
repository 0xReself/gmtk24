using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightPurification : ReceiptCard {
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightShard), 100, 0), 
            new Recipe.ItemBatch(typeof(AzuriteEssence), 20, 0),
            new Recipe.ItemBatch(typeof(AmethystEssence), 10, 0),
            new Recipe.ItemBatch(typeof(CrimsonEssence), 5, 0),
            new Recipe.ItemBatch(typeof(EmberEssence), 3, 0),
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightEssence), 50, 3), 
            new Recipe.ItemBatch(typeof(MoonlightEssence), 0, 4) 
        };

		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
