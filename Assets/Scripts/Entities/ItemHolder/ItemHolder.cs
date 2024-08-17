using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{

	// the connection sides of an item holder (per default left is input and right is output and this is also the default rotation!)
	// a side can be closed, or input, or output
	public enum ConnectionSide
	{
		closed,
		input,
		output,
	}

	[SerializeField]
	// this returns an array with the side configuration from left to right with the default rotation! 
	// this starts at the top left corner (to the left) clockwise to the bottom left corner (to the left) 
	//
	// this is used for getConnectionSides together with the rotation
	private ConnectionSide[] defaultConnections = new ConnectionSide[0];


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
		return new ConnectionSide[0];
	}


	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	public virtual bool canAcceptItem(Item item, int connectionSidePosition)
	{
		return true;
	}

	// An item is given to this item holder (sub class can add custom behaviour, but must return super.acceptItem)
	// 
	// this resets the remaining process time of the item 
	public virtual void acceptItem(Item item, int connectionSidePosition)
	{

	}


	// can be overridden if for example this item holder needs at least n and m items of a specific type to be able to process 
	public virtual bool canProcessItems()
	{
		return true; 
	}

	// can be overridden if this item holder should destroy input items 
	public virtual bool destroyInputs()
	{
		return false;
	}

	// process all items in this item holder (sub class can add custom behaviour, but must call super.processItems)
	public virtual void processItems()
	{
		foreach (Item item in items)
		{
			if (item.canBeProcessed())
			{
				item.process(processingSpeed);
			}
			if(item.isProcessed())
			{
				
			}
		}
	}

	void Start()
	{

	}

	void Update()
	{

	}
}
