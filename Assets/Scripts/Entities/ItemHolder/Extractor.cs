using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Extractor : ItemHolder
{
	[SerializeField]
	private GameObject itemPrefab;


	override protected void onUpdate()
	{
		if(this.items.Count < 1 && GetComponent<Placeable>().isAlive())
		{
			bool success = this.spawnItem(itemPrefab, true);
        }
	}

}
