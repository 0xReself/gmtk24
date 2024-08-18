using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Merger : ItemHolder
{
	// queue of holders that want to send items to this from different sides (will cycle through them)
	private List<ItemHolder> pendingHolders = new List<ItemHolder>();
	private int nextInput = 0;

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		if(otherHolder != null)
		{
			if (pendingHolders.Contains(otherHolder) == false) { 
				pendingHolders.Add(otherHolder);
			}
			for (int i = 0; i < pendingHolders.Count; i++)
			{
				ItemHolder holder = pendingHolders[i];
				if(holder.isAlive() == false)
				{
					pendingHolders.RemoveAt(i--);
					nextInput--;
					if (nextInput <= 0)
					{
						nextInput = 0; 
					}
					continue;
				}
				if(holder == otherHolder && nextInput != i)
				{
					return false;
				}
			}

		}
		bool canAccept = base.canAcceptItem(item, connectionSidePosition, otherHolder);
        if (canAccept)
        {
			nextInput++;
			if (nextInput >= pendingHolders.Count)
			{
				nextInput = 0;
			}
		}
		return canAccept;
	}

}
