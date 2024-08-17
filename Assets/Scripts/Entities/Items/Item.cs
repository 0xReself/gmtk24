using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Item : MonoBehaviour
{

	[SerializeField]
	// The amount of time it takes for item holders to process this item  
	private int processingTime = 0;

	// the remaining time it takes to process the item within the current item holder 
	private int remainingProcessingTime = 0;

	// in which holder the item is currently in
	private ItemHolder holder;


	// resets the processing time of this item (when it arrives at a new item holder). 

	// this can also 
	public virtual void move(ItemHolder holder)
	{
		remainingProcessingTime = processingTime;
		this.holder = holder;
	}

	// called by the holder to proccess the item
	public virtual void process(int speed)
	{
		if(holder!=null)
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
		return holder != null && remainingProcessingTime > 0;
	}

	// returns if the item was processed by the holder. (sub class can add custom behaviour, but must return super.isProcessed) 
	public virtual bool isProcessed()
	{
		return holder != null && remainingProcessingTime <= 0;
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
}
