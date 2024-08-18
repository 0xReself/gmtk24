using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonlightEnrichment : ReceiptCard
{
    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightFragment), 25, 0), 
            new Recipe.ItemBatch(typeof(AzuriteEssence), 5, 0), 
            new Recipe.ItemBatch(typeof(AmethystEssence), 5, 0), 
            new Recipe.ItemBatch(typeof(CrimsonEssence), 5, 0), 
            new Recipe.ItemBatch(typeof(EmberEssence), 5, 0)
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(MoonlightShard), 20, 3), 
            new Recipe.ItemBatch(typeof(MoonlightShard), 0, 4) 
        };
		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
