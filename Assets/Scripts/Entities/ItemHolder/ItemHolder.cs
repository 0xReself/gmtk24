using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{

	// the connection sides of an item holder (per default left is input and right is output and this is also the default rotation!)
	enum ConnectionSide
	{
		left,
		right,
		bot,
		top
	}

	[SerializeField]
	// with default rotation, used for [getInputSides]
	private List<ConnectionSide> defaultInputs = new List<ConnectionSide> {};

	[SerializeField]
	// with default rotation, used for [getOutputSides]
	private List<ConnectionSide> defaultOutputs = new List<ConnectionSide> { };

	[SerializeField]
	// Maximum amount of items this entity can contain at the same time 
	private int maxItems = 0;

	[SerializeField]
	// how fast the item holder processes the item (also depends on the items processing time) 
	private int processingSpeed = 0;

	// the items currently inside of this itemholder
	protected List<Item> items = new List<Item> { };

	public List<ConnectionSide> getInputSides()
	{

	}

	public List<ConnectionSide> getOutputSides()
	{

	}

	// if this item holder can currently accept another items (sub class can add custom behaviour, but must return super.canAcceptItem) 
	public virtual bool canAcceptItem(Item item, ConnectionSide side)
	{

	}

	// An item is given to this item holder (sub class can add custom behaviour, but must return super.acceptItem)
	// 
	// this resets the remaining process time of the item 
	public virtual void acceptItem(Item item, ConnectionSide side)
	{

	}

	// process all items in this item holder (sub class can add custom behaviour, but must call super.processItems)
	public virtual void processItems()
	{

	}

	void Start()
	{

	}

	void Update()
	{

	}
}
