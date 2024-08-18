using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonPurification : ReceiptCard {

    void Start() {
        List <Recipe.ItemBatch> inputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(CrimsonFragment), 2, 0), 
            new Recipe.ItemBatch(typeof(CrimsonShard), 10, 0),
            new Recipe.ItemBatch(typeof(AzuriteEssence), 3, 0),
        };

		List <Recipe.ItemBatch> outputItems = new List<Recipe.ItemBatch> { 
            new Recipe.ItemBatch(typeof(CrimsonEssence), 5, 3), 
            new Recipe.ItemBatch(typeof(CrimsonEssence), 0, 4) 
        };

		recipe = new Recipe(inputItems, outputItems, 100);
    }

    void Update() {
        
    }
}
