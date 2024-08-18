using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// contains all the item prefabs
public class ItemManager : MonoBehaviour
{

	// Fragment
	public GameObject antCoinPrefab;
	public GameObject azuriteFragmentPrefab;
	public GameObject amethystFragmentPrefab;
	public GameObject crimsonFragmentPrefab;
	public GameObject emberFragmentPrefab;
	public GameObject moonlightFragmentPrefab;

	// Shard
	public GameObject azuriteShardPrefab;
	public GameObject amethystShardPrefab;
	public GameObject crimsonShardPrefab;
	public GameObject emberShardPrefab;
	public GameObject moonlightShardPrefab;

	// Essence
	public GameObject azuriteEssencePrefab;
	public GameObject amethystEssencePrefab;
	public GameObject crimsonEssencePrefab;
	public GameObject emberEssencePrefab;
	public GameObject moonlightEssencePrefab;

	// Resource Amounts
	public int fragmentResourceAmount;
	public int shardResourceAmount;
	public int essenceResourceAmount;

	void Start()
	{

	}

	void Update() 
	{ 

	}

	// returns a prefab for the item class (can be null if not found) 
	public GameObject getItemPrefab(Type itemClass)
	{
		if (itemClass == typeof(AntCoin) ) return antCoinPrefab;

		// Fragments
		if (itemClass == typeof(AzuriteFragment) ) return azuriteFragmentPrefab; 
		if (itemClass == typeof(AmethystFragment) ) return amethystFragmentPrefab;
		if (itemClass == typeof(CrimsonFragment)) return crimsonFragmentPrefab;
		if (itemClass == typeof(EmberFragment)) return emberFragmentPrefab;
		if (itemClass == typeof(MoonlightFragment)) return moonlightFragmentPrefab;

		// Shards
		if (itemClass == typeof(AzuriteShard)) return azuriteShardPrefab;
		if (itemClass == typeof(AmethystShard)) return amethystShardPrefab;
		if (itemClass == typeof(CrimsonShard)) return crimsonShardPrefab;
		if (itemClass == typeof(EmberShard)) return emberShardPrefab;
		if (itemClass == typeof(MoonlightShard)) return moonlightShardPrefab;

		// Essence 
		if (itemClass == typeof(AzuriteEssence)) return azuriteEssencePrefab;
		if (itemClass == typeof(AmethystEssence)) return amethystEssencePrefab;
		if (itemClass == typeof(CrimsonEssence)) return crimsonEssencePrefab;
		if (itemClass == typeof(EmberEssence)) return emberEssencePrefab;
		if (itemClass == typeof(MoonlightEssence)) return moonlightEssencePrefab;

		Debug.Log("Did not find matching item prefab for " + itemClass);
		return null; 
	}

	// returns the resource for the item and how much it gives depending on the item
	public (Resource, int) getResourceFromItem(Item item)
	{
		if (item is AmethystShard) return (Resource.Amethyst, shardResourceAmount);
		if (item is AzuriteShard) return (Resource.Azurite, shardResourceAmount);
		if (item is CrimsonShard) return (Resource.Crimson, shardResourceAmount);
		if (item is EmberShard) return (Resource.Ember, shardResourceAmount);
		if (item is MoonlightShard) return (Resource.Moonlight, shardResourceAmount);

		if (item is AmethystFragment) return (Resource.Amethyst, fragmentResourceAmount);
		if (item is AzuriteFragment) return (Resource.Azurite, fragmentResourceAmount);
		if (item is CrimsonFragment) return (Resource.Crimson, fragmentResourceAmount);
		if (item is EmberFragment) return (Resource.Ember, fragmentResourceAmount);
		if (item is MoonlightFragment) return (Resource.Moonlight, fragmentResourceAmount);

		if (item is AmethystEssence) return (Resource.Amethyst, essenceResourceAmount);
		if (item is AzuriteEssence) return (Resource.Azurite, essenceResourceAmount);
		if (item is CrimsonEssence) return (Resource.Crimson, essenceResourceAmount);
		if (item is EmberEssence) return (Resource.Ember, essenceResourceAmount);
		if (item is MoonlightEssence) return (Resource.Moonlight, essenceResourceAmount);

		if (item is AntCoin) return (Resource.ALL, shardResourceAmount + fragmentResourceAmount + essenceResourceAmount);

		Debug.Log("Did not find matching resource " + item);
		return (Resource.NONE, 0); // needs default return 
	}
	
}
