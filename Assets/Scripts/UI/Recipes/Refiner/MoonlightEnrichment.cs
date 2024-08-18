using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightEnrichment : ReceiptCard
{
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightFragment), 1000, 0), 
            new Recipe.ItemBatch(typeof(AzuriteEssence), 100, 0), 
            new Recipe.ItemBatch(typeof(AmethystEssence), 100, 0), 
            new Recipe.ItemBatch(typeof(CrimsonEssence), 100, 0), 
            new Recipe.ItemBatch(typeof(EmberEssence), 100, 0)
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightShard), 10, 3), 
            new Recipe.ItemBatch(typeof(MoonlightShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
