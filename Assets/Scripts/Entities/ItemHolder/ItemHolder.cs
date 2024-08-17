using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.WSA;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class ItemHolder : MonoBehaviour
{

	// the connection sides of an item holder (per default left is input and right is output and top/bot are closed and this is also the default rotation!)
	// a side can be closed, or input, or output
	public enum ConnectionSide
	{
		closed,
		input,
		output,
	}

	public class TargetInformation
	{
		public ItemHolder targetHolder;
		public int myOutputSide;
		public int otherInputSide;
		public TargetInformation(ItemHolder targetHolder, int myOutputSide, int otherInputSide)
		{
			this.targetHolder = targetHolder;
			this.myOutputSide = myOutputSide;
			this.otherInputSide = otherInputSide;
		}
	}

	private MapManager mapManager;

	[SerializeField]
	// this returns an array with the side configuration from left to right with the default rotation! 
	// this starts at the top left corner (to the left) clockwise to the bottom left corner (to the left) 
	//
	// this is used for getConnectionSides together with the rotation
	private ConnectionSide[] defaultConnections = new ConnectionSide[0];

	// used for performances gains
	private ConnectionSide[] cachedConnections;

	// cached to cycle through output sides
	private int currentOutputSide = 0;

	[SerializeField]
	// Maximum amount of items this entity can contain at the same time 
	private int maxItems = 0;

	[SerializeField]
	// how fast the item holder processes the item (also depends on the items processing time) 
	private int processingSpeed = 0;

	// the items currently inside of this itemholder
	protected List<Item> items = new List<Item> { };

	// returns the connection side configuration with the current rotation
	// this starts at the top left corner (to the left) clockwise to the bottom left corner (to the left) 
	public ConnectionSide[] getConnectionSides()
	{
		if(cachedConnections == null)
		{
			Placeable placeable = this.GetComponent<Placeable>();
			if (placeable == null)
			{
				Debug.LogError("Item Holder had no placeable attached!");
				return new ConnectionSide[0];
			}
			else
			{
				int shift = placeable.GetSize() * placeable.GetRotation();
				cachedConnections = new ConnectionSide[defaultConnections.Length];
				for (int i = 0; i < defaultConnections.Length; ++i)
				{
					cachedConnections[(i + shift) % defaultConnections.Length] = defaultConnections[i];
				}
			}
		}
		return cachedConnections;
	}


	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	//
	// for example conveyor belts may want to override this, because they have multiple inputs, but only one output, but cant connect to multiple other belts (for example edges must connect!)
	// or crafting machines may only accept specific items
	public virtual bool canAcceptItem(Item item, int connectionSidePosition)
	{
		return items.Count < maxItems && getConnectionSides()[connectionSidePosition] == ConnectionSide.input;
	}

	// An item is given to this item holder (sub class can add custom behaviour, but must return super.acceptItem)
	// 
	// this is called from the items move function
	// also calls resetTargetForItem to set a new target for the item if possible (otherwise it will be calculated later in update)
	public virtual void acceptItem(Item item, int connectionSidePosition)
	{
		resetTargetForItem(item);
		items.Add(item);
	}

	// spawns the given item as if this item holder produced it if it has space for it and returns true
	//
	// otherwise returns false if the holder has no space for the item
	//
	// the itemprefab will also be instantiated at the current position of this itemHolder
	public bool spawnItem(GameObject itemPrefab, bool isVisible)
	{
		if (items.Count > maxItems || itemPrefab == null)
		{
			return false;
		}
		GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
		if(isVisible == false)
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

	// cycles through the output sides of the current item holder. this returns -1 if there are no output sides at all
	public int getNextOutputSide()
	{
		ConnectionSide[] connections = getConnectionSides();
		currentOutputSide++;
		for (int i = currentOutputSide; i< currentOutputSide + connections.Length; ++i)
		{
			if (connections[i % connections.Length] == ConnectionSide.output)
			{
				currentOutputSide = i % connections.Length;
				return currentOutputSide;
			}
		}
		return -1;
	}

	// this is used to get the next item holder for the processed items and checks if it can accept an item. this can return null if none is found
	// 
	// per default this just circles through the connection output sides if there is a connected holder on that side!!!). returns the holder with the connection side index!
	//
	// can also be overridden in subclass, but it would be better to override processItems instead
	public virtual TargetInformation getNextOutputItemHolder()
	{
		Placeable placeable = GetComponent<Placeable>();
		int size = placeable.GetSize();
		Vector2Int position = placeable.startPosition; // start tile of this is top left field
		int outputSide = getNextOutputSide(); // output side starts top left to the left and end bottom left to the left (clockwise) 
		if (outputSide < 0 || outputSide > getConnectionSides().Length)
		{
			return null; 
		}
		int direction = (outputSide + (size-1) % (size * 4)) / size; // 0=left, 1=top, 2=right, 3=bot
		int steps = (outputSide + size - 1) % size; // clockwise: top: 0,1,2  right : 0,1,2   bot: 0,2,1   left 0,1,2 
		int xOffset = 0;
		int yOffset = 0;

		switch (direction)
		{
			case 0: //left
				xOffset = -1;
				yOffset = steps +1 - size;
				break;
			case 1: // top 
				yOffset = 1;
				xOffset = steps;
				break;
			case 2: // right 
				xOffset = size;
				yOffset = -steps;
				break;
			case 3: // bot
				yOffset = -size;
				xOffset = size - steps - 1;
				break;
		}

		Vector2Int targetPos = new Vector2Int(position.x + xOffset, position.y + yOffset); // where the other item holder should be!
		setMapManager();
		GameObject target = mapManager.Get(targetPos);
		if(target != null)
		{
			ItemHolder otherHolder = target.GetComponent<ItemHolder>();
			if (otherHolder != null)
			{
				int otherSize = target.GetComponent<Placeable>().GetSize();
				int otherInputSide = 0;
				switch (direction)
				{
					case 0: //left to right 
						xOffset = -1;
						yOffset = steps + 1 - size;
						break;
					case 1: // top to bot 
						yOffset = 1;
						xOffset = steps;
						break;
					case 2: // right to left 
						xOffset = size;
						yOffset = -steps;
						break;
					case 3: // bot to top 
						yOffset = -size;
						xOffset = size - steps - 1;
						break;
				}
				if(items.Count > 0 && otherHolder.canAcceptItem(items.ElementAt(0), otherInputSide))
				{
					return new TargetInformation(otherHolder, outputSide, otherInputSide);
				}
			}
		}
		return null;
	}

	// can be overridden if for example this item holder needs at least n and m items of a specific type to be able to process 
	public virtual bool canProcessItems()
	{
		return true; 
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
			if(item.hasTarget() == false)
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
                if (item.hasTarget())
                {
					items.Remove(item);
					--i;
					item.moveToTarget();
					Debug.Log("Item Moved from Holder " + this.ToString() + ": " + item.ToString());
				}
            }
		}
	}

	// used in update to recalculate and initially when accepting the item
	//
	// calls getNextOutputItemHolder and THIS MAY SET THE target of the item to null if there is no valid holder connected!
	private void resetTargetForItem(Item item)
	{
		TargetInformation info = getNextOutputItemHolder();
		if (info != null)
		{
			item.setNewTarget(info.targetHolder, info.myOutputSide, info.otherInputSide);
			Debug.Log("set new target for item: " + this);
		}
		else
		{
			item.setNewTarget(null, getNextOutputSide(), 0);
		}
	}

	void Start()
	{
		setMapManager();
		onStart();
	}

	void Update()
	{
		setMapManager();
		if (canProcessItems())
		{
			processItems();
		}
		onUpdate();
	}

	private void setMapManager()
	{
		if (mapManager == null)
		{
			mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // buggy, why unity? 
		}
	}

	// map manager is still null here
	protected virtual void onStart()
	{

	}

	protected virtual void onUpdate()
	{

	}

	public override string ToString()
	{

		return "ItemHolder{itemCount: " + items.Count + ", current output: " + currentOutputSide + " of " +
			string.Join(",", getConnectionSides()) + ", processingSpeed: " + processingSpeed + ", maxItems: " + maxItems +  "}";
	}
}
