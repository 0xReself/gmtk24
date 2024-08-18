using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Recipe;
using static UnityEditor.Progress;

// recipes used inside of crafters as configuration templates 
public class Recipe
{
	public class ItemBatch
	{
		// the specific item runtime type used as input, or output (will be compared with GetType)
		public Type itemClass;
		// amount of this item that is used for the craft, or produced by it 
		public int itemCount;
		// the connection side of the machine used for this item 
		// the sides start in the top left corner and go clockwise to the bottom left 
		// this is currently only used for output materials! FOR 2x2 machines this will mostly be 3, or 4! 
		public int connectionSide;

		public ItemBatch(Type itemClass, int itemCount, int connectionSide)
		{
			this.itemClass = itemClass;
			this.itemCount = itemCount;
			this.connectionSide = connectionSide;
		}

		// if the item count of the item class is contained inside of the items 
		public bool isContainedIn(List<Item> items)
		{
			int count = 0;
			foreach (Item item in items)
			{
				if(item.GetType() == itemClass)
				{
					count++;
					if (count >= itemCount)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override string ToString()
		{
			return GetType().Name + "{item: " + itemClass + " count: " + itemCount + " side: " + connectionSide + "}";
		}

	}

	// required for one crafting step 
	public List<ItemBatch> inputItems = new List<ItemBatch> { };
	// outputted from one crafting step. IMPORTANT: if only one output resource is used, but you want it on both output slots, add the same item class twice with different output slots, but once with no amount
	public List<ItemBatch> outputItems = new List<ItemBatch> { };
	// the amount of steps it takes the crafter to create this recipe per second
	public float processingTime;

	public Recipe(List<ItemBatch> inputItems, List<ItemBatch> outputItems, float processingTime)
	{
		this.inputItems = inputItems;
		this.outputItems = outputItems;
		this.processingTime = processingTime;
	}

	// all ingredients added together
	public int getTotalInputCount()
	{
		int count = 0;
		foreach (ItemBatch itemBatch in inputItems)
		{
			count += itemBatch.itemCount;
		}
		return count;
	}

	// all output mats added together
	public int getTotalOutputCount()
	{
		int count = 0;
		foreach (ItemBatch itemBatch in outputItems)
		{
			count += itemBatch.itemCount;
		}
		return count;
	}

	// returns if the items contain enough of the item class and may not contain any more (for the crafting recipe)
	// false if not found 
	public bool inputFullOf(List<Item> items, Type itemClass)
	{
		ItemBatch batch = getBatchForItemClass(itemClass, inputItems);
		if(batch == null)
		{
			Debug.LogError("item batch " + batch + " did not contain the item class " + itemClass);
			return false;
		}
		return batch.isContainedIn(items);
	}

	// returns the connection side of the output batch related to the type of the item
	// MOST OF THE TIMES THIS ONLY CONTAINS ONE ENTRY, but if there is only one output mat then two slots can be declared for it
	// empty list if not found 
	public List<int> getOutputSideForItem(Item item)
	{
		Type itemClass = item.GetType();
		List<int> outputSides = new List<int>(); 

		foreach (ItemBatch batch in outputItems)
		{
			if (batch.itemClass == itemClass)
			{
				outputSides.Add(batch.connectionSide);
			}
		}

		if (outputSides.Count <= 0)
		{
			Debug.LogError("item batches of " + this + "did not contain the item " + item);
		}

		return outputSides;
	}

	public ItemBatch getBatchForItemClass(Type itemClass, List<ItemBatch> batches)
	{
		foreach (ItemBatch batch in batches)
		{
			if (batch.itemClass == itemClass)
			{
				return batch;
			}
		}
		return null;
	}

	public override string ToString()
	{
		return GetType().Name + "{processingTime: " + processingTime + " inputItems: " + string.Join(",", inputItems) + " and outputItems: " + string.Join(",", outputItems) + "}";
	}

}
