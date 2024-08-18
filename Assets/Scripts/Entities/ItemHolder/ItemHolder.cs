using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.StandaloneInputModule;
using static UnityEngine.GraphicsBuffer;

public class ItemHolder : MonoBehaviour
{

	// the connection sides of an item holder (per default left is input and right is output and top/bot are closed and this is also the default rotation!)
	// a side can be closed, or input, or output
	public enum ConnectionSide
	{
		// IMPORTANT: belts can have multiple side inputs that are ignored by a splitter, but not by other stuff
		sideInput,
		// the input for everything
		input,
		// no connection
		closed,
		// output for everything
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

	private static MapManager mapManager;

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
			Placeable placeable = getPlaceable();
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
	public virtual bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		return items.Count < maxItems && side <= ConnectionSide.input; // all input kinds, also [ConnectionSide.sideInput] !!!!!!
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

	private Vector2Int calculateMyTargetPos(int direction, int steps, int size, Vector2Int position)
	{
		int xOffset = 0;
		int yOffset = 0;

		switch (direction)
		{
			case 0: //left
				xOffset = -1;
				yOffset = steps + 1 - size;
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
		return new Vector2Int(position.x + xOffset, position.y + yOffset); // where the other item holder should be! (NOT TOP LEFT CORNER)
	}

	private int calculateOtherInputPos(int direction, int steps, int size, Vector2Int position, Placeable otherPlaceable)
	{
		int otherSize = otherPlaceable.GetSize();
		Vector2Int positionDifference = otherPlaceable.startPosition - position; // COMPARED WITH TOP LEFT CORNER OF OTHER ITEM HOLDER. distance is added to the steps of previous holder to get the new one
		int otherSteps = size - steps - 1; // first invert steps, because side is mirrored
		int otherDirection = 0;

		switch (direction)
		{
			case 2: // right to left 
				otherSteps -= positionDifference.y; // negative difference is added as positive
				otherSteps += otherSize - size; // add size difference shift positive if other is bigger
				otherDirection = 4; // special case, in reality this is side 0
				break;
			case 1: // top to bot 
				otherSteps -= positionDifference.x; // same as above, just with x
				otherSteps += otherSize - size;
				otherDirection = 3;
				break;
			case 0: //left to right 
				otherSteps += positionDifference.y; // here only the positive position difference needs to be added
				otherDirection = 2;
				break;
			case 3: // bot to top 
				otherSteps += positionDifference.x; // same as above, just with x
				otherDirection = 1;
				break;
		}
		int paddedSize = otherSize - 1;
		int otherInputSidePos = (otherDirection * otherSize - paddedSize + otherSteps) % (otherSize * 4);
		Debug.Log("other size: " + otherSize + " distance: " + positionDifference + " othersteps: " + otherSteps + " otherdirection: " + otherDirection + " final input pos: " + otherInputSidePos);
		return otherInputSidePos;
	}

	// this is used to get the next item holder for the processed items and checks if it can accept an item. this can return null if none is found
	// 
	// per default this just circles through the connection output sides if there is a connected holder on that side!!!). returns the holder with the connection side index!
	//
	// can also be overridden in subclass, but it would be better to override processItems instead
	public virtual TargetInformation getNextOutputItemHolder()
	{
		Placeable placeable = getPlaceable();
		int size = placeable.GetSize();
		Vector2Int position = placeable.startPosition; // start tile of this is top left field
		int outputSide = getNextOutputSide(); // output side starts top left to the left and end bottom left to the left (clockwise) 
		if (outputSide < 0 || outputSide > getConnectionSides().Length)
		{
			return null; 
		}
		int direction = (outputSide + size - 1) % (size * 4) / size; // 0=left, 1=top, 2=right, 3=bot
		int steps = (outputSide + size - 1) % size; // clockwise: top: 0,1,2  right : 0,1,2   bot: 0,2,1 (right to left)   left 0,1,2 (dowwn to up) 
		
		Vector2Int targetPos = calculateMyTargetPos(direction, steps, size, position);
		ItemHolder otherHolder = getItemHolderAt(targetPos);
		Debug.Log("found target: " + targetPos + " and holder " + otherHolder);

		if (otherHolder != null)
		{
			Placeable otherPlaceable = otherHolder.getPlaceable();
			if (otherPlaceable != null && otherPlaceable.isAlive())
			{
				int otherInputPos = calculateOtherInputPos(direction, steps, size, position, otherPlaceable);
				if (items.Count > 0 && otherHolder.canAcceptItem(items.ElementAt(0), otherInputPos, this))
				{
					return new TargetInformation(otherHolder, outputSide, otherInputPos);
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

	// this is called when the placeable accosiated with this item holder is destroyed/deleted 
	//
	// OVERRIDE THIS IN A SUB CLASS, BUT CALL THE SUPER METHOD! 
	public virtual void onDelete()
	{
		foreach(Item item in items)
		{
			item.delete();
		}
		items.Clear();
	}

	void Start()
	{
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

	public Placeable getPlaceable()
	{
		return GetComponent<Placeable>();
	}

	public static ItemHolder getItemHolderAt(Vector2Int targetPos)
	{
		if (mapManager == null)
		{
			mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // buggy, why unity? 
		}
		GameObject target = mapManager.Get(targetPos);
		if (target != null)
		{
			return target.GetComponent<ItemHolder>();
		}
		return null;
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
		Placeable placeable = getPlaceable();
		if (placeable == null)
		{
			Debug.LogError("trying to log a item holder that did not have a placeable attached");
		}
		return "ItemHolder{itemCount: " + items.Count + ", current output: " + currentOutputSide + " of " +
			string.Join(",", getConnectionSides()) + ", processingSpeed: " + processingSpeed + ", maxItems: " + maxItems +  ", position: " + placeable.startPosition + "}";
	}
}
