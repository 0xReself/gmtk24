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

	protected override void playProcessingAnimation(Item item)
	{
		Vector3 sourcePos = item.getSourcePos();
		Vector3 targetPos = item.getTargetPos();
		Vector3 jumpPoint = sourcePos + (new Vector3(targetPos.x, targetPos.y + 2, 0) - sourcePos) / 2;
		// todo: temp for testing, animation could be better
		if (item.getProgress() <= 0.5f)
		{
			item.transform.position = Vector3.Lerp(sourcePos, jumpPoint, item.getProgress() / 0.5f );
		} 
		else
		{
			item.transform.position = Vector3.Lerp(jumpPoint, targetPos, (item.getProgress() - 0.5f) / 0.5f);
		}
	}

}
