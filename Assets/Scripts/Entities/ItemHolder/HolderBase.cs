using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// base class for itemholder that handles all the target finding and some other base functions
public class HolderBase : MonoBehaviour
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
	private static ItemManager itemManager;

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

	// set to true on start and to false again on delete
	private bool alive = false;

	// returns the connection side configuration with the current rotation
	// this starts at the top left corner (to the left) clockwise to the bottom left corner (to the left) 
	public ConnectionSide[] getConnectionSides()
	{
		if (cachedConnections == null)
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


	public int getNextOutputSide()
	{
		ConnectionSide[] connections = getConnectionSides();
		currentOutputSide++;
		for (int i = currentOutputSide; i < currentOutputSide + connections.Length; ++i)
		{
			if (connections[i % connections.Length] == ConnectionSide.output)
			{
				currentOutputSide = i % connections.Length;
				return currentOutputSide;
			}
		}

		return -1;
	}

	// per default cycles through the output sides of the current item holder. this returns -1 if there are no output sides at all
	// can be adjusted to for example return specific outputs for specific items, etc 
	public virtual int getTargetOutputSideForItem(Item item)
	{
		int output = getNextOutputSide();
		if (output == -1)
		{
			if (hasNoOutput() == false)
			{
				Debug.LogError("holder " + this + " did not have outputs defined for the item " + item);
			}
		}
		return output;
	}

	// absolute number, default 1. overridden for jump pad holder, because there its 2
	protected virtual int estimatedDistanceToTarget()
	{
		return 1;
	}

	private Vector2Int calculateMyTargetPos(int direction, int steps, int size, Vector2Int position)
	{
		int xOffset = 0;
		int yOffset = 0;
		int distance = estimatedDistanceToTarget();

		switch (direction)
		{
			case 0: //left
				xOffset = -distance;
				yOffset = steps + 1 - size;
				break;
			case 1: // top 
				yOffset = distance;
				xOffset = steps;
				break;
			case 2: // right 
				xOffset = size + (distance - 1);
				yOffset = -steps;
				break;
			case 3: // bot
				yOffset = -size - (distance - 1);
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
		///Debug.Log("other size: " + otherSize + " distance: " + positionDifference + " othersteps: " + otherSteps + " otherdirection: " + otherDirection + " final input pos: " + otherInputSidePos);
		return otherInputSidePos;
	}

	// this is used to get the next item holder for the processed items and checks if it can accept an item. this can return null if none is found
	// 
	// per default this just circles through the connection output sides if there is a connected holder on that side!!!). returns the holder with the connection side index!
	//
	// can also be overridden in subclass, but it would be better to override processItems instead
	public virtual TargetInformation getNextOutputItemHolder(Item item, int outputSide)
	{
		if (hasNoOutput())
		{
			return null; // for example sink
		}
		Placeable placeable = getPlaceable();
		int size = placeable.GetSize();
		Vector2Int position = placeable.startPosition; // start tile of this is top left field

		if (outputSide < 0 || outputSide > getConnectionSides().Length)
		{
			Debug.LogError("invalid output size for position: " + position + " at side: " + outputSide + " and holder " + this);
			return null;
		}
		int direction = (outputSide + size - 1) % (size * 4) / size; // 0=left, 1=top, 2=right, 3=bot
		int steps = (outputSide + size - 1) % size; // clockwise: top: 0,1,2  right : 0,1,2   bot: 0,2,1 (right to left)   left 0,1,2 (dowwn to up) 

		Vector2Int targetPos = calculateMyTargetPos(direction, steps, size, position);
		ItemHolder otherHolder = getItemHolderAt(targetPos);

		 /// Debug.Log("found target: " + targetPos + " and holder " + otherHolder  + " while this pos is " + position);

		if (otherHolder != null)
		{
			Placeable otherPlaceable = otherHolder.getPlaceable();
			if (otherPlaceable != null && otherPlaceable.isAlive())
			{
				int otherInputPos = calculateOtherInputPos(direction, steps, size, position, otherPlaceable);
				if (item != null && this is ItemHolder && otherHolder.canAcceptItem(item, otherInputPos, this as ItemHolder))
				{
					return new TargetInformation(otherHolder, outputSide, otherInputPos);
				}
			}
		}

		return null;
	}


	// used in update to recalculate and initially when accepting the item
	//
	// calls getNextOutputItemHolder and THIS MAY SET THE target of the item to null if there is no valid holder connected!
	protected virtual void resetTargetForItem(Item item)
	{
		int outputSide = getTargetOutputSideForItem(item); // output side starts top left to the left and end bottom left to the left (clockwise) 
		TargetInformation info = getNextOutputItemHolder(item, outputSide);
		if (info != null)
		{
			item.setNewTarget(info.targetHolder, info.myOutputSide, info.otherInputSide);
			///Debug.Log("set new target for item: " + this);
		}
		else
		{
			item.setNewTarget(null, outputSide, 0);
		}
	}

	public Placeable getPlaceable()
	{
		return GetComponent<Placeable>();
	}

	public static MapManager getMapManager()
	{
		if (mapManager == null)
		{
			mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>(); // buggy, why unity? 
		}
		return mapManager;
	}

	public static ItemManager getItemManager()
	{
		if (itemManager == null)
		{
			itemManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<ItemManager>();
		}
		return itemManager;
	}

	public static ItemHolder getItemHolderAt(Vector2Int targetPos)
	{
		GameObject target = getMapManager().Get(targetPos);
		if (target != null)
		{
			return target.GetComponent<ItemHolder>();
		}
		return null;
	}

	public virtual void onDelete()
	{
		alive = false;
	}

	public bool isAlive()
	{
		return alive;
	}
	// turns this alive
	public void awake()
	{
		alive = true;
	}

	// can be overridden to supress errors for the sink or similar stuff that has no output
	public virtual bool hasNoOutput()
	{
		return false;
	}

	// used in item holder
	public override string ToString()
	{
		return ", current output: " + currentOutputSide;
	}
}
