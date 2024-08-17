using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.WSA;
using static UnityEditor.Progress;

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
					cachedConnections[i + shift % defaultConnections.Length] = defaultConnections[i];
				}
			}
		}
		return cachedConnections;
	}


	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	//
	// for example conveyor belts may want to override this, because they have multiple inputs, but only one output, but cant connect to multiple other belts (for example edges must connect!)
	public virtual bool canAcceptItem(Item item, int connectionSidePosition)
	{
		return items.Count <= maxItems && getConnectionSides()[connectionSidePosition] == ConnectionSide.input;
	}

	// An item is given to this item holder (sub class can add custom behaviour, but must return super.acceptItem)
	// 
	// this is called from the items move function
	// also calls resetTargetForItem
	public virtual void acceptItem(Item item, int connectionSidePosition)
	{
		resetTargetForItem(item);
		items.Add(item);
	}

	// spawns the given item as if this item holder produced it if it has space for it and returns true
	//
	// otherwise returns false if the holder has no space for the item
	public bool spawnItem(Item item)
	{
		if(items.Count > maxItems)
		{
			return false; 
		}
		item.setNewTarget(this, -1, 0);
		item.moveToTarget();
		resetTargetForItem(item);
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
				currentOutputSide = i;
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
		int outputSide = getNextOutputSide();
		if(outputSide < 0 || outputSide > getConnectionSides().Length)
		{
			return null; 
		}
		Vector2Int position = GetComponent<Placeable>().startPosition;




		return new TargetInformation(null, outputSide, 0);


		//
		//mapManager.Get();
		//
		//
		//
		//canAcceptItem 
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

				item.transform.position = this.transform.position; // todo: temp for testing

			}
			if (item.isProcessed())
			{
                if (item.hasTarget())
                {
					items.Remove(item);
					--i;
					item.moveToTarget();
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
		}
		else
		{
			item.setNewTarget(null, getNextOutputSide(), 0);
		}
	}

	void Start()
	{
		mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
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

	protected virtual void onStart()
	{

	}

	protected virtual void onUpdate()
	{

	}
}
