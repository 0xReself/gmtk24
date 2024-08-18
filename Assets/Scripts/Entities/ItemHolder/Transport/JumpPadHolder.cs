using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JumpPadHolder : ItemHolder
{

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		ConnectionSide side = getConnectionSides()[connectionSidePosition];
		if (otherHolder is Splitter && side == ConnectionSide.sideInput)
		{
			return false;
		}
		return base.canAcceptItem(item, connectionSidePosition, otherHolder);
	}

	protected override int estimatedDistanceToTarget()
	{
		return 2;
	}

}
