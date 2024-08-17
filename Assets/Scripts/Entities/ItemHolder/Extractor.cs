using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.WSA;
using static UnityEditor.Progress;

public class Extractor : ItemHolder
{
	[SerializeField]
	private GameObject itemPrefab;


	override protected void onUpdate()
	{
		if(this.items.Count < 1 && GetComponent<Placeable>().animationFinished())
		{
			bool success = this.spawnItem(itemPrefab, true);
        }
	}

}
