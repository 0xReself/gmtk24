using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemHolder : HolderBase
{
	[SerializeField]
	// Maximum amount of items this entity can contain at the same time. for crafters this is a multiplier to the recipe output count and does not affect the input items!!!!
	protected int maxItems = 0;

	[SerializeField]
	// how fast the item holder processes the item (also depends on the items processing time). set in steps per second
	protected float processingSpeed = 0;

	// the items currently inside of this itemholder. (for crafters this is the input item queue, otherwise its input and output)
	protected List<Item> items = new List<Item> { };


	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	//
	// for example conveyor belts may want to override this, because they have multiple inputs, but only one output, but cant connect to multiple other belts (for example edges must connect!)
	// or crafting machines may only accept specific items
	public virtual bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		return isAlive() && isInputFull() == false && side <= ConnectionSide.input; // all input kinds, also [ConnectionSide.sideInput] !!!!!!
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
	// PER DEFAULT ITEMS ARE ONLY PROCESSED IF THEY HAVE A TARGET!!!
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

			if (item.canBeProcessed() && item.hasTarget() && item.canMoveToTarget())
			{
				item.process(processingSpeed * Time.deltaTime);
				playProcessingAnimation(item);
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
		return isAlive();
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

	// if this item holder contains at least one item to output. only overridden in crafter
	public virtual bool containsOneOutputItem()
	{
		return items.Count > 0;
	}

	// this is called when the placeable accosiated with this item holder is destroyed/deleted 
	//
	// OVERRIDE THIS IN A SUB CLASS, BUT CALL THE SUPER METHOD! 
	public override void onDelete()
	{
		base.onDelete();
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
			Item item = items.ElementAt(i--);
			Debug.Log("deleting item: " + item);
			item.delete();
		}
	}

	// spawns the given item as if this item holder produced it if it has space for it and returns true
	//
	// otherwise returns false if the holder has no space for the item
	//
	// the itemprefab will also be instantiated at the current MIDDLE POSITION of this itemHolder to display the item! (see getMiddlePos) 
	// the initial z position of the item (in the 3d vector) can also be set additionally
	public virtual bool spawnItem(GameObject itemPrefab, bool isVisible, float itemZPos = 0)
	{
		if (isOutputFull() || itemPrefab == null)
		{
			return false;
		}
		GameObject newItem = Instantiate(itemPrefab, new Vector3(getMiddlePos().x, getMiddlePos().y, itemZPos), Quaternion.identity);
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

		Debug.Log("New Item spawned in " + this + " the item : " + item.ToString());
		addSpawnedItem(item);
		return true;
	}

	// this can change the behaviour how the newly spawned item is added to this if overridden in a subclass.
	// per default the items spawn animation is played, item is added to the list with its processing time 
	protected virtual void addSpawnedItem(Item item)
	{
		item.setSource(this, 0, -1, 0, item.getMaxProcessingTime());
		items.Add(item);
		playSpawnAnimation(item);
	}

	// same as spawnItem, but uses ItemManager to look up the prefab for the item class
	public bool spawnItemClass(Type itemClass, bool isVisible, float itemZPos = 0)
	{
		return spawnItem(getItemManager().getItemPrefab(itemClass), isVisible, itemZPos);
	}

	// called once when an item is spawned inside of an holder (it has no target yet, but its own holder as a source!) 
	protected virtual void playSpawnAnimation(Item item)
	{

	}

	// called once when an item is moving from its source to its target holder (before targeting is set and the target holder accepted this item) 
	public virtual void playStartMoveAnimation(Item item)
	{

	}

	// called periodically inside of the current itemholders progress function if the item can be progressed. its animating the move to the next target
	protected virtual void playProcessingAnimation(Item item)
	{
		// todo: temp for testing, animation could be better
		item.transform.position = Vector3.Lerp(item.getSourcePos(), item.getTargetPos(), item.getProgress());
	}

	public override string ToString()
	{
		if (getPlaceable() == null)
		{
			Debug.LogError("trying to log a item holder that did not have a placeable attached");
			return GetType().Name + "{INVALID}";
		}
		return GetType().Name + "{itemCount: " + items.Count + base.ToString() + " of " +
			string.Join(",", getConnectionSides()) + ", processingSpeed: " + processingSpeed + ", maxItems: " + maxItems +  ", position: " + getTopLeftCorner() + "}";
	}

	public float getProcessingSpeed()
	{
		return processingSpeed;
	}
}
