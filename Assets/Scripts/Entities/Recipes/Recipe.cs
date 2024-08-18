using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// recipes used inside of crafters as configuration templates 
public class Recipe
{
	public class ItemBatch
	{
		// the specific item runtime type used as input, or output 
		Type itemClass;
		// amount of this item that is used for the craft, or produced by it 
		int itemCount;
		// the connection side of the machine used for this item 
		// the sides start in the top left corner and go clockwise to the bottom left 
		// this is currently only used for output materials! 
		int connectionSide;
	}

	// required for one crafting step 
	public List<ItemBatch> inputItems = new List<ItemBatch> { };
	// outputted from one crafting step 
	public List<ItemBatch> outputItems = new List<ItemBatch> { };
	// the amount of time it takes the crafter to create this recipe 
	public int processingTime;

	public Recipe(List<ItemBatch> inputItems, List<ItemBatch> outputItems, int processingTime)
	{
		this.inputItems = inputItems;
		this.outputItems = outputItems;
		this.processingTime = processingTime;
	}
}
