using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public enum Resource {
    Azurite = 0,
    Amethyst = 1,
    Ember = 2,
    Crimson = 3,
    Moonlight = 4,
    NONE = 5,
    ALL = 6,
}

public class ResourceManager : MonoBehaviour {
    
    [SerializeField]
    private int azuriteEssence = 0;

    [SerializeField]
    private int amethystEssence = 0;

    [SerializeField]
    private int emberEssence = 0;

    [SerializeField]
    private int crimsonEssence = 0;

    [SerializeField]
    private int moonlightEssence = 0;

    public void AddResource(Resource resource, int amount) {
        switch (resource) {
            case Resource.Amethyst:
                amethystEssence += amount;
                break;

            case Resource.Ember:
                emberEssence += amount;
                break;

            case Resource.Crimson:
                crimsonEssence += amount;
                break;

            case Resource.Moonlight:
                moonlightEssence += amount;
                break;

			case Resource.Azurite:
				azuriteEssence += amount;
				break;

			case Resource.ALL:
				amethystEssence += amount;
				emberEssence += amount;
				crimsonEssence += amount;
				moonlightEssence += amount;
				azuriteEssence += amount;
				break;

			default:
				break;
		}
    }

    public int GetResource(Resource resource) {
        switch (resource) {
            case Resource.Amethyst:
                return amethystEssence;

            case Resource.Ember:
                return emberEssence;

            case Resource.Crimson:
                return crimsonEssence;

            case Resource.Moonlight:
                return moonlightEssence;

			case Resource.Azurite:
				return azuriteEssence;

			default:
                return 0;
        }
    }

    public bool RemoveResource(Resource resource, int amount) {
        switch (resource) {
            case Resource.Amethyst:
                if (amount > amethystEssence) {
                    return false;
                } 
                amethystEssence -= amount;
                return true;
                
            case Resource.Ember:
                if (amount > emberEssence) {
                    return false;
                }
                emberEssence -= amount;
                return true;

            case Resource.Crimson:
                if (amount > crimsonEssence) {
                    return false;
                }
                crimsonEssence -= amount;
                return true;

            case Resource.Moonlight:
                if (amount > moonlightEssence) {
                    return false;
                }
                moonlightEssence -= amount;
                return true;

			case Resource.Azurite:
				if (amount > azuriteEssence)
				{
					return false;
				}
				azuriteEssence -= amount;
				return true;

			case Resource.ALL:
				azuriteEssence -= amount;
				azuriteEssence -= amount;
				azuriteEssence -= amount;
				azuriteEssence -= amount;
				return true;

			default:
				return false;
		}
    }

    public bool CanRemove(Resource resource, int amount) {
        switch (resource) {
            case Resource.Amethyst:
                if (amount > amethystEssence) {
                    return false;
                } 
                return true;
                
            case Resource.Ember:
                if (amount > emberEssence) {
                    return false;
                }
                return true;

            case Resource.Crimson:
                if (amount > crimsonEssence) {
                    return false;
                }
                return true;

            case Resource.Moonlight:
                if (amount > moonlightEssence) {
                    return false;
                }
                return true;

			case Resource.Azurite:
				if (amount > azuriteEssence)
				{
					return false;
				}
				return true;

			case Resource.ALL:
				return true;

			default:
				return false;
		}
    }

    void Start() {
        
    }

    
    void Update() {
        
    }
}
