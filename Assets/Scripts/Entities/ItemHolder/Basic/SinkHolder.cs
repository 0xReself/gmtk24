using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// very high max items and processing speed, because it deletes all items. also has inputs on all 6x6 sides
public class SinkHolder : ItemHolder
{

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		return base.canAcceptItem(item, connectionSidePosition, otherHolder);
	}


	public override void processItems()
	{
		for (int i = 0; i < items.Count; ++i)
		{
			Item item = items[i];

			if (item.canBeProcessed())
			{
				item.process(processingSpeed * Time.deltaTime);
				// todo: temp for testing
				item.transform.position = Vector3.Lerp(item.transform.position, getMiddlePos(), processingSpeed / 10.0f * Time.deltaTime);

			}
			if (item.isProcessed())
			{
				items.Remove(item); // does not need a target here
				item.delete();
				(Resource resource, int amount) = getItemManager().getResourceFromItem(item);
				getResourceManager().AddResource(resource, amount);
				Debug.Log("Item converted to " + amount +  " of resource " + resource + " inside of " + this + ": " + item.ToString());
			}
		}
	}

	public override bool hasNoOutput()
	{
		return true;
	}

	public ResourceManager getResourceManager()
	{
		return GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
	}

}
