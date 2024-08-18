using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// crafter uses the normal list of items as inputs for the crafts
public class Crafter : ItemHolder
{

	// the current recipe of the crafter. this can also be null 
	private Recipe recipe;

	// this is the list of output items this crafter produces! it stores the output amount of the recipe multiplied by [maxItems]
	protected List<Item> outputItems = new List<Item> { };

	// this is set from the recipe and displays the remaining time it takes to craft 
	private float remainingProcessTime = 0;

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		if (recipe == null || base.canAcceptItem(item, connectionSidePosition, otherHolder) == false)
		{
			return false; // still check base function (which already uses overridden isfull methods from below)! 
		}

		if(recipe.inputFullOf(items, item.GetType()))
		{
			return false; // only accept items of this type until we have enough for the craft
		}

		return true; 
	}

	public override bool canProcessItems()
	{
		if (recipe == null || base.canProcessItems() == false)
		{
			return false; // still check base
		}
		return true; 
	}

	public override void processItems()
	{
		for (int i = 0; i < items.Count; ++i)
		{
			Item item = items[i];
			if (item.canBeProcessed())
			{
				item.process(processingSpeed * Time.deltaTime);
				// todo: temp for testing, maybe instantly make them vanish? 
				item.transform.position = Vector3.Lerp(item.transform.position, this.transform.position, processingSpeed / 10.0f * Time.deltaTime);

			}
		}

		for (int i = 0; i < outputItems.Count; ++i)
		{
			Item item = outputItems[i];
			if (item.hasTarget() == false && (item.canBeProcessed() || item.isProcessed()))
			{
				resetTargetForItem(item);
			}

			if (item.canBeProcessed())
			{
				item.process(processingSpeed * Time.deltaTime);
				// todo: temp for testing, maybe instantly spawn them on the next belt? 
				item.transform.position = Vector3.Lerp(item.transform.position, this.transform.position, processingSpeed / 10.0f * Time.deltaTime);

			}
			if (item.isProcessed())
			{
				if (item.hasTarget() && item.canMoveToTarget())
				{
					outputItems.Remove(item);
					--i;
					item.moveToTarget();
					Debug.Log("Item Moved from Crafter " + this + ": " + item.ToString());
				}
			}

		}

		if (readyToCraft())
		{
			if (isProcessing())
			{
				process();
			}
            else
            {
				finishCraft();
			}
        }

	}

	private void process()
	{
		remainingProcessTime -= processingSpeed * Time.deltaTime;
		// todo: maybe play some animation while crafting? 
	}

	// this also needs to reset everything
	private void finishCraft()
	{
		remainingProcessTime = recipe.processingTime;
		deleteAllItems();
		foreach (Recipe.ItemBatch batch in recipe.outputItems)
		{
			for (int i = 0; i < batch.itemCount; ++i)
			{
				spawnItemClass(batch.itemClass, true); // todo: maybe not visibile output items
			}
		}
	}

	public override int getTargetOutputSideForItem(Item item)
	{
		int targetSide = recipe.getOutputSideForItem(item);
		ConnectionSide[] connections = getConnectionSides();
		for (int i = 0; i < connections.Length; ++i)
		{
			if (connections[i] == ConnectionSide.output && i == targetSide)
			{
				return i;
			}
		}
		Debug.LogError("crafter " + this + " did not find a target for the item " + item);
		return -1;
	}

	private bool readyToCraft()
	{
		return isInputFull() == true && isOutputFull() == false; // can craft only if the input is full and the output is not full
	}

	private bool isProcessing()
	{
		return remainingProcessTime > 0.0000000001f;
	}

	// overridden to compare input items (old item list) with the recipe input count
	protected override bool isInputFull()
	{
		return items.Count >= getMaxInputItems();
	}

	// overridden to compare new output items and also count the recipe output
	protected override bool isOutputFull()
	{
		return outputItems.Count >= getMaxOutputItems();
	}
	public int getMaxInputItems()
	{
		if(recipe == null)
		{
			return 0;
		}
		return recipe.getTotalInputCount();
	}

	// IMPORTANT: multiply recipe outputcount with maxitems!
	public int getMaxOutputItems()
	{
		if (recipe == null)
		{
			return 0;
		}
		return recipe.getTotalOutputCount() * maxItems;
	}

	public void setRecipe(Recipe recipe)
	{
		this.recipe = recipe;
		remainingProcessTime = recipe.processingTime;
		deleteAllItems();
		deleteAllOutputItems();
	}


	// this can change the behaviour how the newly spawned item is added to this if overridden in a subclass
	protected override void addSpawnedItem(Item item)
	{
		item.setSource(this, 0, -1, 0, 10.0f); // todo: very very short remaining processing time / delay? 
		outputItems.Add(item);
	}

	public override void onDelete()
	{
		base.onDelete();
		deleteAllOutputItems();
	}

	// deletes all output items that are not already deleted from the default deleteallitems function
	public void deleteAllOutputItems()
	{
		for (int i = 0; i < outputItems.Count; ++i)
		{
			outputItems.ElementAt(i--).delete();
		}
		outputItems.Clear();
	}

	public override bool containsOneOutputItem()
	{
		return outputItems.Count > 0;
	}

}
