using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Recipe;

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

	}

	// required for one crafting step 
	public List<ItemBatch> inputItems = new List<ItemBatch> { };
	// outputted from one crafting step 
	public List<ItemBatch> outputItems = new List<ItemBatch> { };
	// the amount of time it takes the crafter to create this recipe 
	public double processingTime;

	public Recipe(List<ItemBatch> inputItems, List<ItemBatch> outputItems, int processingTime)
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
	public bool inputFullOf(List<Item> items, Type itemClass)
	{
		foreach(ItemBatch batch in inputItems)
		{
			if (batch.itemClass == itemClass)
			{
				return batch.isContainedIn(items);
			}
		}
		return false;
	}

}
