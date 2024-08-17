using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{

	[SerializeField]
	// The amount of time it takes for item holders to process this item  
	private int processingTime = 0;

	// the remaining time it takes to process the item within the current item holder 
	private int remainingProcessingTime = 0;

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
	// this also calls acceptitem on the next holder 
	public void moveToTarget()
	{
		nextOutputHolder.acceptItem(this, connectedTargetInputSide);
		remainingProcessingTime = processingTime;
		this.currentHolder = nextOutputHolder;
		this.inputSide = connectedTargetInputSide;
		this.nextOutputHolder = null;
		this.outputSide = 0;
		this.connectedTargetInputSide = 0;
	}

	// sets a new target item holder where the item will move to next
	public void setNewTarget(ItemHolder target, int outputSide, int connectedTargetInputSide)
	{
		this.nextOutputHolder = target;
		this.outputSide = outputSide;
		this.connectedTargetInputSide = connectedTargetInputSide;
	}

	// called by the holder to proccess the item
	public virtual void process(int speed)
	{
		if(currentHolder != null)
		{
			if(remainingProcessingTime > 0)
			{
				remainingProcessingTime -= speed;
			}
		}
		else
		{
			Debug.LogError("Item process had no holder!");
		}
	}

	// returns if the item can be processed by the holder. (sub class can add custom behaviour, but must return super.canBeProcessed) 
	public virtual bool canBeProcessed()
	{
		return currentHolder != null && remainingProcessingTime > 0;
	}

	// returns if the item was processed by the holder. (sub class can add custom behaviour, but must return super.isProcessed) 
	public virtual bool isProcessed()
	{
		return currentHolder != null && remainingProcessingTime <= 0;
	}

	// if the item has a new holder where it will move to 
	public bool hasTarget()
	{
		return nextOutputHolder != null;
	}

	public void delete()
	{
		Destroy(this.gameObject);
	}


	void Start()
	{

	}

	void Update()
	{

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
}
