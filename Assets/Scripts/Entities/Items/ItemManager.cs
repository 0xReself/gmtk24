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

	public GameObject azuriteEssencePrefab;
	public GameObject amethystEssencePrefab;
	public GameObject crimsonEssencePrefab;
	public GameObject emberEssencePrefab;
	public GameObject moonlightEssencePrefab;

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

		return null; 
	}
	
}
