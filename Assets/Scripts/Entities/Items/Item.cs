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


	// resets the processing time of this item (when it arrives at a new item holder) 
	public void reset()
	{
		remainingProcessingTime = processingTime;
	}


	void Start()
	{

	}

	void Update()
	{

	}
}
