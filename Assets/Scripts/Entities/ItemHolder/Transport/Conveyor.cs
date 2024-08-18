using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : ItemHolder
{

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		if(otherHolder is Splitter && side == ConnectionSide.sideInput)
		{
			return false;
		}
		return base.canAcceptItem(item, connectionSidePosition, otherHolder);
	}

}
