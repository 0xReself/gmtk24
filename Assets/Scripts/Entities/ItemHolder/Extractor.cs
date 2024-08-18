using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Extractor : ItemHolder
{
	[SerializeField]
	private GameObject itemPrefab;

	[SerializeField]
	bool ableToGenerate = false;

	private Vector2Int getInputRotation(int rotation) {
		switch (rotation) {
			case 1:
				return new Vector2Int(0, 1);

			case 2:
				return new Vector2Int(1, 0);

			case 3:
				return new Vector2Int(0, -1);

			default:
				return new Vector2Int(-1, 0);
		}
	}

	void Start() {
		Placeable placeable = GetComponent<Placeable>();

		Vector2Int corePosition = getInputRotation(placeable.rotation);
		Vector2Int position = placeable.startPosition;


		GameObject coreObject = getMapManager().Get(position + corePosition);
		if (coreObject != null) {
			if(coreObject.GetComponent<Core>() != null) {
				ableToGenerate = true;
			}
		}
	}

	override protected void onUpdate() {
		if(this.items.Count < 1 && GetComponent<Placeable>().isAlive() && ableToGenerate)
		{
			bool success = this.spawnItem(itemPrefab, true);
        }
	}

}
