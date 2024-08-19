using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Merger : ItemHolder
{

	// the target next holder which can only supply to this ( when set to -1, it accepts all holders) 
	private int nextHolderNum = -1;

	// queue of holders that want to send items to this from different sides (will cycle through them)
	private List<ItemHolder> pendingHolders = new List<ItemHolder>();

	public override bool canAcceptItem(Item item, int connectionSidePosition, ItemHolder otherHolder)
	{
		if(otherHolder != null)
		{
			if (pendingHolders.Contains(otherHolder) == false) { 
				pendingHolders.Add(otherHolder);
			}
			int myPosition = 0;
			for (int i = 0; i < pendingHolders.Count; i++)
			{
				ItemHolder holder = pendingHolders[i];
				if(holder.isAlive() == false)
				{
					if (nextHolderNum == i)
					{
						nextHolderNum = -1; 
					}
					else if(nextHolderNum > i)
					{
						nextHolderNum--;
					}
					pendingHolders.RemoveAt(i--);
				}
				else if(holder == otherHolder )
				{
					myPosition = i;
					if (nextHolderNum >= 0 && nextHolderNum != myPosition)
					{
						return false;
					}
				}
            }

		} 
		else
		{
			return false; 
		}
		bool canAccept = base.canAcceptItem(item, connectionSidePosition, otherHolder);
        if (canAccept)
        {
			for (int i = nextHolderNum + 1; i < pendingHolders.Count + nextHolderNum; i++)
			{
				int index = i % pendingHolders.Count;
				ItemHolder holder = pendingHolders[index];
				if (holder.isAlive() && holder.containsOneOutputItem())
				{
					nextHolderNum = index;
					return canAccept;
				}
			}
			nextHolderNum = -1;
		}
		return canAccept;
	}

}
