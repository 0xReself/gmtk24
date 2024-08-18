using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
	[SerializeField]
	// The amount of time it takes for item holders to process this item. this is ignored for the crafter. amount of steps it takes to move an item per second
	private float processingTime = 0;

	// the remaining time it takes to process the item within the current item holder 
	private float remainingProcessingTime = 0;

	// in which holder the item is currently in
	private ItemHolder currentHolder;

	// the next target holder where the item will move to (can be null and may be updated when placing other item holder)
	private ItemHolder nextOutputHolder;

	// input of current holder where item came from
	private int inputSide = 0;
	// output of current holder where item goes to
	private int outputSide = 0;
	// connected to the output side, is the input side of the target
	private int connectedTargetInputSide = 0;


	// resets the processing time of this item (when it arrives at a new item holder). and shifts from current to next item holder 
	//
	// this also calls acceptitem on the next holder which also calls setNewTarget
	public void moveToTarget()
	{
		setSource(nextOutputHolder, connectedTargetInputSide, 0, 0, processingTime);
		if(currentHolder== null)
		{
			Debug.LogError("Item did not have a next holder");
			delete();
		}
		else
		{
			currentHolder.acceptItem(this, connectedTargetInputSide);
		}
	}

	// sets a new target item holder where the item will move to next (this will be called from the acceptItem from the item holder through moveToTarget)
	public void setNewTarget(ItemHolder target, int outputSide, int connectedTargetInputSide)
	{
		this.nextOutputHolder = target;
		this.outputSide = outputSide;
		this.connectedTargetInputSide = connectedTargetInputSide;
	}

	// as an alternative to calling setnewtarget and movetotarget twice, this explecitly sets a source for the item (used in crafter, because it has a different item queue)
	// per default inputside should be 0 and outputside -1   and the connectedtargetinputside is 0
	// the remainingProcessingTime can also be overridden here!
	public void setSource(ItemHolder source, int inputSide, int outputside, int connectedTargetInputSide, float remainingProcessingTime)
	{
		this.remainingProcessingTime = remainingProcessingTime;
		this.currentHolder = source;
		this.nextOutputHolder = null;
		this.inputSide = inputSide;
		this.outputSide = outputside;
		this.connectedTargetInputSide = connectedTargetInputSide;
	}

	// called by the holder to proccess the item
	public virtual void process(float speed)
	{
		if(currentHolder != null)
		{
			if(remainingProcessingTime > 0.0f)
			{
				remainingProcessingTime -= speed;
			}
		}
		else
		{
			Debug.LogError("Item process had no holder!");
		}
	}

	// checked before moving and before setting the target in getNextOutputItemHolder 
	public virtual bool canMoveToTarget()
	{
		return nextOutputHolder != null && nextOutputHolder.canAcceptItem(this, this.connectedTargetInputSide, currentHolder);
	}

	// returns if the item can be processed by the holder. (sub class can add custom behaviour, but must return super.canBeProcessed) 
	public virtual bool canBeProcessed()
	{
		return currentHolder != null && remainingProcessingTime > 0.0000000001f;
	}

	// returns if the item was processed by the holder. (sub class can add custom behaviour, but must return super.isProcessed) 
	public virtual bool isProcessed()
	{
		return currentHolder != null && remainingProcessingTime < 0.0000000001f;
	}

	// if the item has a new holder where it will move to 
	public bool hasTarget()
	{
		return nextOutputHolder != null;
	}

	// deletes the item and also removes it from the attached holders if they contain it and also deletes the holder references
	public void delete()
	{
		if(currentHolder != null)
		{
			currentHolder.removeItem(this);
		}
		if (nextOutputHolder != null)
		{
			nextOutputHolder.removeItem(this);
		}
		currentHolder = null; 
		nextOutputHolder = null;
		Destroy(this.gameObject);
	}


	void Start()
	{
		onStart();
	}

	void Update()
	{
		onUpdate();
	}

	// where the item arrived on the current holder
	public int getInputSide()
	{
		return inputSide;
	}

	// the side where the item will move to the next holder
	public int getOutputSide()
	{
		return outputSide;
	}

	protected virtual void onStart()
	{

	}

	protected virtual void onUpdate()
	{

	}

	public override string ToString()
	{
		string current = "null";
		if (currentHolder != null)
		{
			current = currentHolder.ToString();
		}
		string target = "null";
		if(nextOutputHolder != null)
		{
			target = nextOutputHolder.ToString();
		}

		return GetType().Name + "{current holder: " + current + ", next holder: " + target + ", remainingProcessingTime: " + 
			remainingProcessingTime + ", inputSide: " + inputSide + ", outputSide: " + outputSide + ", connectedTargetInputSide: " + connectedTargetInputSide +  "}";
	}
}
