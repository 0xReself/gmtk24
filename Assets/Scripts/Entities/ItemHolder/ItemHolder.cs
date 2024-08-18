using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHolder : HolderBase
{
	// set to true on start and to false again on delete
	private bool alive = false;

	[SerializeField]
	// Maximum amount of items this entity can contain at the same time. for crafters this is a multiplier to the recipe output count and does not affect the input items!!!!
	protected int maxItems = 0;

	[SerializeField]
	// how fast the item holder processes the item (also depends on the items processing time) 
	protected int processingSpeed = 0;

	// the items currently inside of this itemholder. (for crafters this is the input item queue, otherwise its input and output)
	protected List<Item> items = new List<Item> { };


	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	//
	// for example conveyor belts may want to override this, because they have multiple inputs, but only one output, but cant connect to multiple other belts (for example edges must connect!)
	// or crafting machines may only accept specific items
	public virtual bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		return alive && isInputFull() == false && side <= ConnectionSide.input; // all input kinds, also [ConnectionSide.sideInput] !!!!!!
	}

	// An item is given to this item holder (sub class can add custom behaviour, but must return super.acceptItem)
	// 
	// this is called from the items move function
	// also calls resetTargetForItem to set a new target for the item if possible (otherwise it will be calculated later in update)
	public virtual void acceptItem(Item item, int connectionSidePosition)
	{
		items.Add(item);
		resetTargetForItem(item);
		Debug.Log("holder " + this + " accepted new item " + item);
	}

	// process all items in this item holder (sub class can add custom behaviour instead of this to process all items!)
	//
	// this calls otherHolder.acceptItem and removes the item from this holder. also calls resetTargetForItem for those with no target periodically
	//
	// for example an override would destroy input items instead of moving them along 
	public virtual void processItems()
	{
		for (int i = 0; i < items.Count; ++i)
		{
			Item item = items[i];
			if(item.hasTarget() == false && (item.canBeProcessed() || item.isProcessed()))
			{
				resetTargetForItem(item);
			}

			if (item.canBeProcessed())
			{
				item.process(processingSpeed * Time.deltaTime);
				// todo: temp for testing
				item.transform.position = Vector3.Lerp(item.transform.position, this.transform.position, processingSpeed / 10 * Time.deltaTime);

			}
			if (item.isProcessed())
			{
                if (item.hasTarget() && item.canMoveToTarget())
                {
					items.Remove(item);
					--i;
					item.moveToTarget();
					Debug.Log("Item Moved from Holder " + this + ": " + item.ToString());
				}
            }
		}
	}

	// can be overridden if for example this item holder needs at least n and m items of a specific type to be able to process 
	public virtual bool canProcessItems()
	{
		return alive;
	}

	// per default compares item count with max items and is used in canAcceptItem
	protected virtual bool isInputFull()
	{
		return items.Count >= maxItems;
	}

	// per default compares item count with max items and is used in spawnItem
	protected virtual bool isOutputFull()
	{
		return items.Count >= maxItems; 
	}


	// this is called when the placeable accosiated with this item holder is destroyed/deleted 
	//
	// OVERRIDE THIS IN A SUB CLASS, BUT CALL THE SUPER METHOD! 
	public virtual void onDelete()
	{
		alive = false;
		deleteAllItems();
	}

	void Start()
	{
		awake();
		onStart();
	}

	void Update()
	{
		if (canProcessItems())
		{
			processItems();
		}
		onUpdate();
	}

	// map manager is still null here
	protected virtual void onStart()
	{

	}

	protected virtual void onUpdate()
	{

	}

	// only removes the item, does not clear references inside of the item, or deletes it!!!
	public void removeItem(Item item)
	{
		if (items.Contains(item))
		{
			items.Remove(item);
		}
	}

	// deletes all items
	public void deleteAllItems()
	{
		for (int i = 0; i < items.Count; ++i)
		{
			items.ElementAt(i--).delete();
		}
		items.Clear();
	}

	// spawns the given item as if this item holder produced it if it has space for it and returns true
	//
	// otherwise returns false if the holder has no space for the item
	//
	// the itemprefab will also be instantiated at the current position of this itemHolder
	public bool spawnItem(GameObject itemPrefab, bool isVisible)
	{
		if (isOutputFull() || itemPrefab == null)
		{
			return false;
		}
		GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
		if (isVisible == false)
		{
			newItem.GetComponent<Renderer>().enabled = false;
		}
		Item item = newItem.GetComponent<Item>();
		if (item == null)
		{
			Debug.LogError("new item did not have a item component");
			Destroy(newItem);
			return false;
		}

		item.setNewTarget(this, -1, 0);
		item.moveToTarget();
		Debug.Log("New Item spawned: " + item.ToString());
		return true;
	}

	// same as spawnItem, but uses ItemManager to look up the prefab for the item class
	public bool spawnItemClass(Type itemClass, bool isVisible)
	{
		return spawnItem(getItemManager().getItemPrefab(itemClass), isVisible);
	}

	public override string ToString()
	{
		Placeable placeable = getPlaceable();
		if (placeable == null)
		{
			Debug.LogError("trying to log a item holder that did not have a placeable attached");
		}
		return GetType().Name + "{itemCount: " + items.Count + base.ToString() + " of " +
			string.Join(",", getConnectionSides()) + ", processingSpeed: " + processingSpeed + ", maxItems: " + maxItems +  ", position: " + placeable.startPosition + "}";
	}
}
