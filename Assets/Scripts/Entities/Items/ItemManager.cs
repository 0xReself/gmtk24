using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// contains all the item prefabs
public class ItemManager : MonoBehaviour
{

	public GameObject antCoinPrefab;


	void Start()
	{

	}

	void Update() 
	{ 

	}

	// returns a prefab for the item class (can be null if not found) 
	public GameObject getItemPrefab(Type itemClass)
	{
		if( itemClass == typeof(AntCoin) )
		{
			return antCoinPrefab;
		}
		return null; 
	}
	
}
