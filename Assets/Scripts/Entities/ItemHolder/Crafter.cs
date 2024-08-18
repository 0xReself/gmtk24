using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crafter : ItemHolder
{

	// the current recipe of the crafter. this can also be null 
	public Recipe recipe;

	// this is the list of output items this crafter produces! it stores the output amount of the recipe multiplied by [maxItems]
	protected List<Item> outputItems = new List<Item> { };

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		if (recipe == null)
		{
			return false;
		}
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		return items.Count < maxItems && side <= ConnectionSide.input; // not using base method here, because items count is used differntly 
	}

	public override bool canProcessItems()
	{
		if (recipe == null)
		{
			return false;
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
				item.transform.position = Vector3.Lerp(item.transform.position, this.transform.position, processingSpeed / 10 * Time.deltaTime);

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
				item.transform.position = Vector3.Lerp(item.transform.position, this.transform.position, processingSpeed / 10 * Time.deltaTime);

			}
		}
	}


}
