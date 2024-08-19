using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

// base class for itemholder that handles all the target finding and some other base functions
public class HolderBase : MonoBehaviour
{

	static private bool debugLogPosSpamm = false; // toggled to spamm logs 
	static private Type onlyDebugType = typeof(Crafter); // can be null to log everything

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

	// shifts a connection side number according to the rotation of this 
	public int shiftConnectionSide(int notRotatedConnectionSide, int totalConnectionCount)
	{
		Placeable placeable = getPlaceable();
		if (placeable == null)
		{
			Debug.LogError("Item Holder had no placeable attached!");
			return notRotatedConnectionSide;
		}
		int shift = placeable.GetSize() * placeable.GetRotation();
		return (notRotatedConnectionSide + shift) % totalConnectionCount;
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

	// absolute number in jumps to the target (tiles in the diraction), default 1. overridden for jump pad holder, because there its 2
	// used in calculateMyTargetPos
	protected virtual int estimatedDistanceToTarget()
	{
		return 1;
	}

	// clamps an outputside (which should hopefully already be rotated) into two values: 
	// direction as 0=left, 1=top, 2=right, 3=bot
	// steps clamped for the sides clockwise: top: 0,1,2  right : 0,1,2   bot: 0,2,1 (right to left)   left 0,1,2 (dowwn to up) 
	public static (int direction, int steps) clampOutputSide(int outputSide, int size)
	{
		int direction = (outputSide + size - 1) % (size * 4) / size;
		int steps = (outputSide + size - 1) % size;
		return (direction, steps);
	}

	// direction and steps should be retrieved from clampOutputSide. this returns where the other item holder should be! (ITS NOT ALWAYS THE TOP LEFT CORNER OF THE OTHER HOLDER)
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
		return new Vector2Int(position.x + xOffset, position.y + yOffset);
	}

	// similar to clampOutputSide, but here it returns the matching side and step position of the other holder
	// that intersects with the given myDirection and mySteps of this holder! 
	// IMPORTANT: otherPosition is not the target position, IT MUST BE THE TOP LEFT CORNER OF THE other itemholder
	private (int otherDirection, int otherSteps) getClampedOutputSideOfOther(int myDirection, int mySteps, int mySize, Vector2Int myPosition, int otherSize, Vector2Int otherPosition)
	{
		Vector2Int positionDifference = otherPosition - myPosition; // compared from this to other
		int sizeDifference = otherSize - mySize; // also compared from this to other
		int invertedSteps = mySize - mySteps - 1; // first invert steps, because side is mirrored
		int otherDirection = 0;
		int shift = 0; // the step shift depending on the side so that the low side ranges (0..1..2..3..4) match 
		switch (myDirection)
		{
			case 2: // right to left 
				shift = sizeDifference - positionDifference.y;
				// explanation: uses y, because the steps are on the y axis 
				// if other size is bigger (with same pos), then we have to add difference, because the numbers are higher (lowest can not be reached)
				// if other side is higher (same size, but bigger y pos), then we have to substract difference, because the numbers are lower (highest cannot be reached)
				otherDirection = 0; // mirrored direction of the other 
				break;
			case 1: // top to bot 
				shift = sizeDifference + positionDifference.x;// here x axis and shifted to right in positive position should be added to the shift and size as well 
				otherDirection = 3;
				break;
			case 0: //left to right 
				shift = positionDifference.y; // the other two sides are different, because here counting starts from the top and not from the bottom
				// therefor the sizedifference does not matter here! and if other side is lower (same size, but lesser y pos),
				// then we have to add that negative difference, because numbers are lower (highest cannot be reached)
				otherDirection = 2;
				break;
			case 3: // bot to top 
				shift = - positionDifference.x; // here x axis negate position diff, because other further to the left should add number (when pos is less than this) 
				otherDirection = 1;
				break;
		}
		if (canDebugLog())
		{
			Debug.Log("calculated clamped output side of other: positionDiff: " + positionDifference + " sizeDifference: " + sizeDifference + " invertedSteps: " + invertedSteps +
				" otherDirection: " + otherDirection + " shift: " + shift + " otherdirection: " + otherDirection + " otherSteps: " + (invertedSteps + shift));

		}
		return (otherDirection, invertedSteps + shift);
	}

	// THIS returns the matching input connection side position of the target which connects to the output connection side position of this holder!
	// direction and steps should be retrieved from clampOutputSide
	private int calculateOtherInputPos(int direction, int steps, int size, Vector2Int position, ItemHolder otherHolder)
	{
		int otherSize = otherHolder.getSize();
		(int otherDirection, int otherSteps) = getClampedOutputSideOfOther(direction, steps, size, position, otherSize, otherHolder.getTopLeftCorner());
		int sideMultiplier = otherDirection - 1;
		if (sideMultiplier == -1)
		{
			sideMultiplier = 3; // map left side which is 0 to 4 so it can be used in the following calculation and decrease all sides by one 
		}

		int sideStartPos = sideMultiplier * otherSize + 1; // first get to the first number
		int otherInputSidePos = sideStartPos + otherSteps;// then add the steps to get the final target pos 

		int maxPos = otherSize * 4; // cant be bigger than that (this one will be mapped to 0)
		if (otherInputSidePos == maxPos)
		{
			otherInputSidePos = 0; 
		}
		else if(otherInputSidePos < 0 || otherInputSidePos > maxPos || otherSteps < 0 || otherDirection < 0 || otherDirection >= 4)
		{
			Debug.LogError("ERROR CALCULATING target positions: my size: " + size + " my steps: " + steps + " my direction: " + direction +
			" other size: " + otherSize + " othersteps: " + otherSteps + " otherdirection: " + otherDirection +
			" final input pos: " + otherInputSidePos + " myTopLeft: " + getTopLeftCorner() + " otherTopLeft: " + otherHolder.getTopLeftCorner() + " i am: " + this + " and other is: " + otherHolder);
		}

		if (canDebugLog())
		{
			Debug.Log("calculated target positions: my size: " + size + " my steps: " + steps + " my direction: " + direction +
				" other size: " + otherSize + " othersteps: " + otherSteps + " otherdirection: " + otherDirection + 
				" final input pos: " + otherInputSidePos + " myTopLeft: " + getTopLeftCorner() + " otherTopLeft: " + otherHolder.getTopLeftCorner());

		}
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
		int size = getPlaceable().GetSize();
		Vector2Int position = getTopLeftCorner(); // start tile of this is top left field

		if (outputSide < 0 || outputSide > getConnectionSides().Length)
		{
			Debug.LogError("invalid output size for position: " + position + " at side: " + outputSide + " and holder " + this);
			return null;
		}
		(int direction, int steps) = clampOutputSide(outputSide, size);

		Vector2Int targetPos = calculateMyTargetPos(direction, steps, size, position);
		ItemHolder otherHolder = getItemHolderAt(targetPos);

		if(canDebugLog())
		{
			Debug.Log("found target: " + targetPos + " and holder " + otherHolder + " while this pos is " + position);
		}

		if (otherHolder != null)
		{
			Placeable otherPlaceable = otherHolder.getPlaceable();
			if (otherPlaceable != null && otherPlaceable.isAlive())
			{
				int otherInputPos = calculateOtherInputPos(direction, steps, size, position, otherHolder);

				if (item != null && this is ItemHolder && otherHolder.canAcceptItem(item, otherInputPos, this as ItemHolder))
				{
					if (canDebugLog())
					{
						Debug.Log("Target can accept item");

					}
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
			//Debug.Log("set new target for item: " + this);
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

	// uses placeable to return start position! 
	public Vector2Int getTopLeftCorner()
	{
		return getPlaceable().startPosition;
	}

	// just relays it to placeable, should be amount of tiles this occupies in each direction (so 1 for 1x1 and 2 for 2x2, etc)
	public int getSize()
	{
		return getPlaceable().GetSize();
	}

	public Vector2 getMiddlePos()
	{
		return getPlaceable().startPosition; // TODO: EDIT 
	}

	protected virtual bool canDebugLog()
	{
		return debugLogPosSpamm && (onlyDebugType == null || onlyDebugType.IsAssignableFrom(GetType()));
	}
}
